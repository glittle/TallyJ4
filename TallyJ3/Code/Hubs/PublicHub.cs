using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TallyJ3.Code.Hubs
{
    public class PublicHub {
        private readonly IHubContext<PublicHubCore> _coreHub;

        public const string HubName = "Public";

        public PublicHub(IHubContext<PublicHubCore> hub) {
            this._coreHub = hub;
        }

        public void TellPublicAboutVisibleElections()
        {
            var list = new List<string> { "ZZ", "ZZ" };
            //var list = new PublicElectionLister().RefreshAndGetListOfAvailableElections();
            _coreHub.Clients.Group(HubName).SendAsync("ElectionsListUpdated", list);
        }
    }

    public class PublicHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, PublicHub.HubName);
            return base.OnConnectedAsync();
        }
    }
}