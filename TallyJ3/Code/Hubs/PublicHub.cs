using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using TallyJ3.Code.Misc;

namespace TallyJ3.Code.Hubs
{
    public class PublicHubHelper : IPublicHubHelper
    {
        private readonly IHubContext<PublicHubCore> _coreHub;

        public PublicHubHelper(IHubContext<PublicHubCore> publicHubContext) 
        {
            _coreHub = publicHubContext;
        }

        public void TellPublicAboutVisibleElections(string msg = null)
        {
            //var list = new PublicElectionLister().RefreshAndGetListOfAvailableElections();
            var list = new List<string> { "ZZ", "inside helper", msg };
            _coreHub.Clients.All.SendAsync("ElectionsListUpdated", list);
        }
    }

    public interface IPublicHubHelper
    {
        void TellPublicAboutVisibleElections(string msg = null);
    }

    public class PublicHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            //Groups.AddAsync(Context.ConnectionId, GroupNameForPublic);
            return base.OnConnectedAsync();
        }
    }
}