using Microsoft.AspNetCore.Mvc;
using System;
using TallyJ4.Extensions;
using TallyJ4.Models;

namespace TallyJ4.Controllers
{
  [Route("[controller]/[action]")]
  public class PublicController : Controller
  {
    [HttpPost]
    public object TellerJoin(Guid electionGuid, string passcode, Guid? oldCompGuid)
    {
      return new TellerModel().GrantAccessToGuestTeller(electionGuid, passcode, oldCompGuid.AsGuid());
    }
    public object OpenElections()
    {
      return new PublicElectionLister().RefreshAndGetListOfAvailableElections();
    }
  }
}