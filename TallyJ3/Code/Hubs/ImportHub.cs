using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TallyJ3.Code.Session;

namespace TallyJ3.Code.Hubs
{
    public class ImportHub : IStatusUpdateHub
    {
        private IHubContext<ImportHubCore> _coreHub;

        public static string GroupNameForUser
        {
            get
            {
                return "Import" + UserSession.LoginId;
            }
        }

        public void ImportInfo(int linesProcessed, int peopleAdded)
        {
            _coreHub.Clients.Group(GroupNameForUser).SendAsync("ImportInfo", linesProcessed, peopleAdded);
        }

        public void StatusUpdate(string msg, bool msgIsTemp = false)
        {
            _coreHub.Clients.Group(GroupNameForUser).SendAsync("LoaderStatus", msg, msgIsTemp);
        }
    }

    public class ImportHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, ImportHub.GroupNameForUser);
            return base.OnConnectedAsync();
        }
    }
}