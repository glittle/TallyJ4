using Microsoft.AspNetCore.Mvc.RazorPages;
using TallyJ3.Code.Hubs;

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
