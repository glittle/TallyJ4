using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TallyJ3.Code.Session;

namespace TallyJ3.Code.Hubs
{
    public interface IFrontDeskHubHelper
    {
        void UpdatePeople(object message);
    }

    public class FrontDeskHubHelper : IFrontDeskHubHelper
    {

        private IHubContext<FrontDeskHub> HubContext { get; }

        public FrontDeskHubHelper(IHubContext<FrontDeskHub> hubContext)
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

        public void UpdatePeople(object message)
        {
            HubContext.Clients.Group(GroupNameForElection).SendAsync("updatePeople", message);
        }
    }

    public class FrontDeskHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, FrontDeskHubHelper.GroupNameForElection);
            return base.OnConnectedAsync();
        }
    }
}