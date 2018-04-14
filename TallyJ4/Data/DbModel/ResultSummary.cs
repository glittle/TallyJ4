using System;
using System.Collections.Generic;

namespace TallyJ3.Data.DbModel
{
    
    public partial class ResultSummary
    {
        public int Id { get; set; }
        public System.Guid ElectionGuid { get; set; }
        public string ResultType { get; set; }
        public Nullable<bool> UseOnReports { get; set; }
        public Nullable<int> NumVoters { get; set; }
        public Nullable<int> NumEligibleToVote { get; set; }
        public Nullable<int> MailedInBallots { get; set; }
        public Nullable<int> DroppedOffBallots { get; set; }
        public Nullable<int> InPersonBallots { get; set; }
        public Nullable<int> SpoiledBallots { get; set; }
        public Nullable<int> SpoiledVotes { get; set; }
        public Nullable<int> TotalVotes { get; set; }
        public Nullable<int> BallotsReceived { get; set; }
        public Nullable<int> BallotsNeedingReview { get; set; }
        public Nullable<int> CalledInBallots { get; set; }
        public Nullable<int> SpoiledManualBallots { get; set; }
    }
}
