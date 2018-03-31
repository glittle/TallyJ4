using Microsoft.AspNetCore.Mvc.RazorPages;
using TallyJ3.Code.Hubs;

namespace TallyJ3.Pages
{
    public class AboutModel : PageModel
    {
        private readonly PublicHub _publicHub;

        public AboutModel(PublicHub publicHub)
        {
            _publicHub = publicHub;
        }

        public string Message { get; set; }

        public void OnGet()
        {

            Message = "Your application description page.";

            _publicHub.TellPublicAboutVisibleElections();
        }
    }
}
