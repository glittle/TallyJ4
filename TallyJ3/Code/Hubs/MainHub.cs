using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TallyJ3.Code.Session;
using TallyJ3.Models;

namespace TallyJ3.Code.Hubs
{
    public interface IMainHubHelper
    {
        void StatusChanged(object infoForKnown, object infoForGuest);
        void CloseOutGuestTellers();
    }

    public class MainHubHelper : IMainHubHelper
    {
        public IHubContext<MainHub> HubContext { get; }

        public MainHubHelper(IHubContext<MainHub> hubContext)
        {
            HubContext = hubContext;
        }

        public static string GroupNameForElection
        {
            get
            {
                return UserSession.CurrentElectionGuid.ToString();
            }
        }

        public void StatusChanged(object infoForKnown, object infoForGuest)
        {
            HubContext.Clients.Group(GroupNameForElection + "Known").SendAsync("statusChanged", infoForKnown);
            HubContext.Clients.Group(GroupNameForElection + "Guest").SendAsync("statusChanged", infoForGuest);
        }

        public void CloseOutGuestTellers()
        {
            HubContext.Clients.Group(GroupNameForElection + "Guest").SendAsync("electionClosed");
        }
    }
    
    public class MainHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var group = MainHubHelper.GroupNameForElection + (UserSession.IsKnownTeller ? "Known" : "Guest");
            Groups.AddAsync(Context.ConnectionId, group);
            new ComputerModel().RefreshLastContact();
            return base.OnConnectedAsync();
        }
    }
}