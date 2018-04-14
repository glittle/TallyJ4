using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallyJ4.Data;

namespace TallyJ4.Data.DbModel
{
    [Serializable]
    public class OnlineTempBallot : IIndexedForCaching
    {
        public int Id { get; set; }
        public Guid ElectionGuid { get; set; }
        public string PersonGuidList { get; set; }
        public string Status { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
