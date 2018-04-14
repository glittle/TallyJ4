using Microsoft.AspNetCore.Mvc.RazorPages;
using TallyJ4.Code.Hubs;

namespace TallyJ4.Pages
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
