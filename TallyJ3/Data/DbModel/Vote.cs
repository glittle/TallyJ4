using System;

namespace TallyJ3.Data.DbModel
{
    using System.Collections.Generic;
    
    public partial class Vote : IIndexedForCaching
    {
        public int Id { get; set; }
        public System.Guid BallotGuid { get; set; }
        public int PositionOnBallot { get; set; }
        public Nullable<System.Guid> PersonGuid { get; set; }
        public string StatusCode { get; set; }
        public Nullable<System.Guid> InvalidReasonGuid { get; set; }
        public Nullable<int> SingleNameElectionCount { get; set; }
        public byte[] C_RowVersion { get; set; }
        public string PersonCombinedInfo { get; set; }
    }
}
