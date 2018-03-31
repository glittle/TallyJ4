using Microsoft.Extensions.DependencyInjection;
using TallyJ3.Code.Session;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace TallyJ3.EF
{
    public class CacherHelper
    {
        /// <summary>
        ///   Remove all cached data for this election
        /// </summary>
        public void DropAllCachesForThisElection()
        {
            var cache = Startup.ServiceProvider.GetService<IMemoryCache>();

            var electionGuid = UserSession.CurrentElectionGuid.ToString();

            var dict = cache.Get<Dictionary<string, bool>>(electionGuid);

            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    cache.Remove(kvp.Key);
                }
                cache.Remove(electionGuid);
            }
        }
    }
}