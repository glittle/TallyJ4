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
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Your application description page.";

            Startup.GetService<IPublicHubHelper>().TellPublicAboutVisibleElections("local DI");
        }
    }
}
