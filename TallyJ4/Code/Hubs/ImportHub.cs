using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TallyJ4.Code.Session;

namespace TallyJ4.Code.Hubs
{
    public interface IImportHubHelper
    {
        void ImportInfo(int linesProcessed, int peopleAdded);
        void StatusUpdate(string msg, bool msgIsTemp = false);
    }

    public class ImportHubHelper : IStatusUpdateHub, IImportHubHelper
    {
        public ImportHubHelper(IHubContext<ImportHub> hubContext)
        {
            HubContext = hubContext;
        }

        public static string GroupNameForUser
        {
            get
            {
                return UserSession.LoginId;
            }
        }

        public IHubContext<ImportHub> HubContext { get; }

        public void ImportInfo(int linesProcessed, int peopleAdded)
        {
            HubContext.Clients.Group(GroupNameForUser).SendAsync("ImportInfo", linesProcessed, peopleAdded);
        }

        public void StatusUpdate(string msg, bool msgIsTemp = false)
        {
            HubContext.Clients.Group(GroupNameForUser).SendAsync("LoaderStatus", msg, msgIsTemp);
        }
    }

    public class ImportHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddAsync(Context.ConnectionId, ImportHubHelper.GroupNameForUser);
            return base.OnConnectedAsync();
        }
    }
}