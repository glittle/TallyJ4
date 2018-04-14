using Microsoft.AspNetCore.Mvc;
using System;
using TallyJ3.Code.Session;
using TallyJ3.Extensions;

namespace TallyJ3.Controllers
{
  //[Produces("application/json")]
  [Route("[controller]/[action]")]
  public class MiscController : Controller
  {
    [HttpPost]
    public object GetTimeOffset(long now = 0)
    {
      if (now == 0)
      {
        // not called by our code?
        return null;
      }

      // adjust client time by .5 seconds to allow for network and server time
      const double fudgeFactor = .5 * 1000;
      var clientTimeNow = new DateTime(1970, 1, 1).AddMilliseconds(now + fudgeFactor);
      var serverTime = DateTime.Now;
      var diff = (serverTime - clientTimeNow).TotalMilliseconds;
      UserSession.TimeOffsetServerAhead = diff.AsInt();
      UserSession.TimeOffsetKnown = true;
      return new
      {
        timeOffset = diff
      };
    }
  }
}