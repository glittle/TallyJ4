using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TallyJ3.Code.Misc;
using TallyJ3.Code.Session;

namespace TallyJ3.Code.Hubs
{
    public class RollCallHub
    {
        private IHubContext<RollCallHubCore> _coreHub;

        public static string GroupNameForElection
        {
            get
            {
                var electionGuid = UserSession.CurrentElectionGuid;
                AssertAtRuntime.That(electionGuid != Guid.Empty);

                return "RollCall" + electionGuid;
            }
        }

        public void UpdateAllConnectedClients(object message)
        {
            _coreHub.Clients.Group(GroupNameForElection).SendAsync("updatePeople", message);
        }
    }

    public class RollCallHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, RollCallHub.GroupNameForElection);
            return base.OnConnectedAsync();
        }
    }
}