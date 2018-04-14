using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TallyJ3.Code.Session;

namespace TallyJ3.Code.Hubs
{
    public class AnalyzeHubHelper : IAnalyzeHubHelper, IStatusUpdateHub
    {
        public IHubContext<AnalyzeHubCore> HubContext { get; }

        public AnalyzeHubHelper(IHubContext<AnalyzeHubCore> hubContext)
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

        public void StatusUpdate(string msg, bool msgIsTemp = false)
        {
            HubContext.Clients.Group(GroupNameForElection).SendAsync("LoadStatus", msg, msgIsTemp);
        }
    }

    public interface IAnalyzeHubHelper : IStatusUpdateHub
    {
    }

    public class AnalyzeHubCore : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, AnalyzeHubHelper.GroupNameForElection);
            return base.OnConnectedAsync();
        }
    }
}