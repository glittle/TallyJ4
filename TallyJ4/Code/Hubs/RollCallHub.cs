using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TallyJ3.Code.Session;

namespace TallyJ3.Code.Hubs
{
    public interface IRollCallHubHelper
    {
        void UpdateAllConnectedClients(object message);
    }

    public class RollCallHubHelper : IRollCallHubHelper
    {

        public RollCallHubHelper(IHubContext<RollCallHub> hubContext)
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

        public IHubContext<RollCallHub> HubContext { get; }

        public void UpdateAllConnectedClients(object message)
        {
            HubContext.Clients.Group(GroupNameForElection).SendAsync("updatePeople", message);
        }
    }

    public class RollCallHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, RollCallHubHelper.GroupNameForElection);
            return base.OnConnectedAsync();
        }
    }
}