using System.Collections.Concurrent;
using System.Linq;
using System;
using TallyJ4.Data.Caching;
using TallyJ4.Extensions;

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
    public string RefreshAndGetListOfAvailableElections()
    {
      const string template = "<option value=\"{0}\">{1} {2}</option>";

      var activeElectionGuids = new ComputerCacher().ElectionGuidsOfActiveComputers;

      if (activeElectionGuids.Count == 0)
      {
        return template.FilledWith(0, "(No elections are active right now.)", "");
      }

      var elections = GetNewDbContext().Election
        .Where(e => activeElectionGuids.Contains(e.ElectionGuid)
             && e.ListForPublic.HasValue
             && e.ListForPublic.Value
             && e.ElectionPasscode != null)
        .Select(e => new { e.Name, e.ElectionGuid, e.Convenor })
        .ToList();

      return elections
        .OrderBy(e => e.Name)
        .Select(e => template.FilledWith(e.ElectionGuid, e.Name, e.Convenor.SurroundContentWith("(", ")")))
        .JoinedAsString();
    }

  }
}