using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using TallyJ3.Code.Misc;
using TallyJ3.Code.Session;
using TallyJ3.Models;

namespace TallyJ3.Code.Hubs
{
    public class MainHub : IMainHub
    {
        private IHubContext<MainHubCore> _coreHub;

        public MainHub(IHubContext<MainHubCore> hub)
        {
            _coreHub = hub;
        }

        public static string GroupNameForElection
        {
            get
            {
                var electionGuid = UserSession.CurrentElectionGuid;
                AssertAtRuntime.That(electionGuid != Guid.Empty);

                return "Main" + electionGuid;
            }
        }

        public void StatusChanged(object infoForKnown, object infoForGuest)
        {
            _coreHub.Clients.Group(GroupNameForElection + "Known").SendAsync("statusChanged", infoForKnown);
            _coreHub.Clients.Group(GroupNameForElection + "Guest").SendAsync("statusChanged", infoForGuest);
        }

        public void CloseOutGuestTellers()
        {
            _coreHub.Clients.Group(GroupNameForElection + "Guest").SendAsync("electionClosed");
        }
    }

    public interface IMainHub
    {
        void StatusChanged(object infoForKnown, object infoForGuest);
        void CloseOutGuestTellers();
    }

    public class MainHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            var group = MainHub.GroupNameForElection + (UserSession.IsKnownTeller ? "Known" : "Guest");

            Groups.AddAsync(Context.ConnectionId, MainHub.GroupNameForElection);

            new ComputerModel().RefreshLastContact();

            return base.OnConnectedAsync();
        }
    }
}