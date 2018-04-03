using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using TallyJ3.Code.Misc;
using TallyJ3.Code.Session;

namespace TallyJ3.Code.Hubs
{
    public class FrontDeskHub
    {
        private IHubContext<FrontDeskHubCore> _coreHub;

        public static string GroupNameForElection
        {
            get
            {
                var electionGuid = UserSession.CurrentElectionGuid;
                AssertAtRuntime.That(electionGuid != Guid.Empty);

                return "FrontDesk" + electionGuid;
            }
        }

        public void UpdatePeople(object message)
        {
            _coreHub.Clients.Group(GroupNameForElection).SendAsync("updatePeople", message);
        }
    }

    public class FrontDeskHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, FrontDeskHub.GroupNameForElection);
            return base.OnConnectedAsync();
        }
    }
}