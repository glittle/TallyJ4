using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using TallyJ3.Code.Misc;
using TallyJ3.Code.Session;

namespace TallyJ3.Code.Hubs
{
    public class AnalyzeHub : IAnalyzeHub, IStatusUpdateHub
    {
        private IHubContext<AnalyzeHubCore> _coreHub;

        public AnalyzeHub(IHubContext<AnalyzeHubCore> hub)
        {
            _coreHub = hub;
        }

        public static string GroupNameForElection
        {
            get
            {
                var electionGuid = UserSession.CurrentElectionGuid;
                AssertAtRuntime.That(electionGuid != Guid.Empty);

                return "Analyze" + electionGuid;
            }
        }

        public void StatusUpdate(string msg, bool msgIsTemp = false)
        {
            _coreHub.Clients.Group(GroupNameForElection).SendAsync("LoadStatus", msg, msgIsTemp);
        }
    }

    public interface IAnalyzeHub : IStatusUpdateHub
    {
    }

    public class AnalyzeHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, AnalyzeHub.GroupNameForElection);
            return base.OnConnectedAsync();
        }
    }
}