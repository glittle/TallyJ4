using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
//using TallyJ3.EF;

namespace TallyJ3.Core.Hubs
{
    public class PublicHub : Hub
    {
        public const string HubName = "Public";

        /// <summary>
        ///   Join this connection into the hub
        /// </summary>
        /// <param name="connectionId"></param>
        public void Join(string connectionId)
        {
            Groups.AddAsync(connectionId, HubName);
        }

        public void TellPublicAboutVisibleElections()
        {
            //var list = new PublicElectionLister().RefreshAndGetListOfAvailableElections();
            var list = new List<string>{ "L", "L" };
            Clients.Group(HubName).SendAsync("ElectionsListUpdated", list);
        }
    }
}