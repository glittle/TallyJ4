using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TallyJ3.Code.Session;
using TallyJ3.Data.DbModel;
using TallyJ3.Extensions;

namespace TallyJ3.Code.Helper
{
    public interface ILogHelper
    {
        void Add(string message, bool alsoSendToRemoteLog = false);
    }

    public class LogHelper : ILogHelper
    {
        private readonly Guid _electionGuid;

        public LogHelper(Guid electionGuid)
        {
            _electionGuid = electionGuid;
        }

        public LogHelper() : this(UserSession.CurrentElectionGuid)
        {
        }

        public void Add(string message, bool alsoSendToRemoteLog = false)
        {
            AddToLog(new Log
            {
                ElectionGuid = _electionGuid,
                ComputerCode = UserSession.CurrentComputerCode,
                LocationGuid = UserSession.CurrentLocationGuid,
                Details = message
            });
            if (alsoSendToRemoteLog)
            {
                SendToRemoteLog(message);
            }
        }

        public async void SendToRemoteLog(string message)
        {
            var iftttKey = Startup.Configuration["iftttKey"].DefaultTo("");
            if (iftttKey.HasNoContent())
            {
                return;
            }

            var info = new NameValueCollection();
            //TODO
            //info["value1"] = "{0} / {1} / {2}".FilledWith(UserSession.LoginId, Environment.MachineName, HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.Url.Host);
            info["value2"] = UserSession.CurrentElectionName;
            info["value3"] = message;

            var url = "https://maker.ifttt.com/trigger/{0}/with/key/{1}".FilledWith("TallyJ", iftttKey);

            using (var client = new WebClientWithTimeout(500))
            {
                try
                {
                    await Task.Run(() => client.UploadValues(url, info));
                }
                catch (Exception)
                {
                    // ignore if we can't send to remote log
                }
            }
        }

        private void AddToLog(Log logItem)
        {
            var db = UserSession.GetNewDbContext();
            logItem.AsOf = DateTime.Now;
            db.Log.Add(logItem);
            db.SaveChanges();
        }
    }
    public class WebClientWithTimeout : WebClient
    {
        public WebClientWithTimeout(int timeout)
        {
            Timeout = timeout;
        }
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = Timeout;
            return request;
        }
    }
}