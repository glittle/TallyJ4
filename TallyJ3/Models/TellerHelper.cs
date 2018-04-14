using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TallyJ3.Code.Session;
using TallyJ3.Data.Caching;
using TallyJ3.Extensions;

namespace TallyJ3.Models
{
    public class TellerHelper : DataConnectedModel
  {
    public string GetTellerOptions(int tellerIdToSelect)
    {
      var tellerName = UserSession.GetCurrentTeller(tellerIdToSelect);

      return new TellerCacher(GetNewDbContext()).AllForThisElection
        .OrderBy(l => l.Name)
        .Select(l => new { l.Id, l.Name, Selected = l.Name == tellerName ? " selected" : "" })
        .Select(l => "<option value={Id}{Selected}>{Name}</option>".FilledWith(l))
        .JoinedAsString()
        .SurroundWith("<option value='0'>Which Teller?</option>", "<option value='-1'>+ Add my name</option>");
    }
  }
}