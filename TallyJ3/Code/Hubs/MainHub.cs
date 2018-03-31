using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TallyJ3.Code.Session;
using TallyJ3.Code.Misc;
using TallyJ3.Code.Models;
//using TallyJ.Code.Helpers;
//using TallyJ.Code.Session;

namespace TallyJ3.CoreModels.Hubs
{
    public class MainHub
    {
        private IHubContext<MainHubCore> _coreHub;

        public MainHub(IHubContext<MainHubCore> hub)
        {
            this._coreHub = hub;
        }

        public static string HubName
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
            _coreHub.Clients.Group(HubName + "Known").SendAsync("statusChanged", infoForKnown);
            _coreHub.Clients.Group(HubName + "Guest").SendAsync("statusChanged", infoForGuest);
        }

        public void CloseOutGuestTellers()
        {
            _coreHub.Clients.Group(HubName + "Guest").SendAsync("electionClosed");
        }
    }

    public class MainHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            var group = MainHub.HubName + (UserSession.IsKnownTeller ? "Known" : "Guest");

            Groups.AddAsync(Context.ConnectionId, MainHub.HubName);

            new ComputerModel().RefreshLastContact();

            return base.OnConnectedAsync();
        }
    }
}