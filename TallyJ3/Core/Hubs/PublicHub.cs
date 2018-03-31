using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using TallyJ3.EF;

namespace TallyJ3.Core.Hubs
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
            _coreHub.Clients.Group(HubName).SendAsync("ElectionsListUpdated", list);
        }
    }

    //public interface IPublicHub
    //{
    //    void TellPublicAboutVisibleElections();
    //}

    public class PublicHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, PublicHub.HubName);
            return base.OnConnectedAsync();
        }
    }
}