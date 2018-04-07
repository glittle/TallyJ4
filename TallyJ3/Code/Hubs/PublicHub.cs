using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TallyJ3.Code.Hubs
{
    public interface IPublicHubHelper
    {
        void TellPublicAboutVisibleElections(string msg = null);
    }


    public class PublicHubHelper : IPublicHubHelper
    {
        public IHubContext<PublicHub> HubContext { get; }

        public PublicHubHelper(IHubContext<PublicHub> hubContext) 
        {
            HubContext = hubContext;
        }


        public void TellPublicAboutVisibleElections(string msg = null)
        {
            //var list = new PublicElectionLister().RefreshAndGetListOfAvailableElections();
            var list = new List<string> { "ZZ", "inside helper", msg };
            HubContext.Clients.All.SendAsync("ElectionsListUpdated", list);
        }
    }

    public class PublicHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            //Groups.AddAsync(Context.ConnectionId, GroupNameForPublic);
            return base.OnConnectedAsync();
        }
    }
}