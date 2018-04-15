using System.Collections.Concurrent;
using System.Linq;
using System;
using TallyJ4.Data.Caching;
using TallyJ4.Extensions;
using System.Collections.Generic;

namespace TallyJ4.Models
{
  public class PublicElectionLister : DataConnectedModel
  {
    /// <summary>
    /// Is this election Id in the list of publically visible ids?
    /// </summary>
    /// <param name="electionGuid"></param>
    /// <returns></returns>
    public string GetPasscodeIfAvailable(Guid electionGuid)
    {
      var activeElectionGuids = new ComputerCacher().ElectionGuidsOfActiveComputers.Where(g => g == electionGuid).ToList();
      if (activeElectionGuids.Count == 0)
      {
        return null;
      }

      var election = GetNewDbContext().Election
        .FirstOrDefault(e => e.ElectionGuid == electionGuid
                             && e.ListForPublic.HasValue
                             && e.ListForPublic.Value);
      return election == null ? null : election.ElectionPasscode;
    }

    /// <summary>
    /// Refresh the list and return it.
    /// </summary>
    /// <returns></returns>
    public List<ListItem> RefreshAndGetListOfAvailableElections()
    {
      var activeElectionGuids = new ComputerCacher().ElectionGuidsOfActiveComputers;

      var elections = GetNewDbContext().Election
        .Where(e => activeElectionGuids.Contains(e.ElectionGuid)
             && e.ListForPublic.HasValue
             && e.ListForPublic.Value
             && e.ElectionPasscode != null)
        .Select(e => new { e.Name, e.ElectionGuid, e.Convenor })
        .ToList();

      if (elections.Count == 0)
      {
        return new List<ListItem> {
          {
            new ListItem { value= "0", text= "(No elections are active right now.)" }
          }
        };
      }

      return elections
        .OrderBy(e => e.Name)
        .Select(e => new ListItem { value = e.ElectionGuid.ToString(), text = e.Name })
        .ToList();
    }

  }
  public struct ListItem
  {
    public string value { get; set; }
    public string text { get; set; }
  }
}