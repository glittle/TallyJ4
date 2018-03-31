using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using TallyJ3.Core.Hubs;

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
            //new PublicHub().TellPublicAboutVisibleElections();

            var list = new List<string> { "L", "L" };
            //_publicHub.Clients.Group("Public").SendAsync("ElectionsListUpdated", list);

            _publicHub.TellPublicAboutVisibleElections();

            //PublicHub.TellPublicAboutVisibleElections(_publicHub);
        }
    }
}
