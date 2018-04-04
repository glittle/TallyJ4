using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using TallyJ3.Code.Hubs;
using TallyJ3.Code.Misc;

namespace TallyJ3.Pages
{
    public class AboutModel : PageModel
    {
        private readonly IHubContext<PublicHubCore> _publicHubContext;
        private readonly IPublicHubHelper _helper;

        public AboutModel(IHubContext<PublicHubCore> publicHubContext, IPublicHubHelper helper)
        {
            _publicHubContext = publicHubContext;
            _helper = helper;
        }

        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Your application description page.";

            // from injected
            var list = new List<string> { "ZZ", "in hub" };
            _publicHubContext.Clients.All.SendAsync("ElectionsListUpdated", list);

            // from DI
            var publicHub = Startup.ServiceProvider.GetRequiredService<IHubContext<PublicHubCore>>();
            publicHub.Clients.All.SendAsync("ElectionsListUpdated", "Hello");

            // encapsulated in SharedEnvironment
            SharedEnvironment.Current.PublicHubHelper.TellPublicAboutVisibleElections("shared");

            // helper
            _helper.TellPublicAboutVisibleElections("helper");

            // from DI
            Startup.ServiceProvider.GetRequiredService<IPublicHubHelper>().TellPublicAboutVisibleElections("local DI");
        }
    }
}
