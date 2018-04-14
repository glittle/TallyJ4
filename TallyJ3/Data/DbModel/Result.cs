using System;

namespace TallyJ3.Data.DbModel
{
    public partial class Result : IIndexedForCaching
    {
        public int Id { get; set; }
        public System.Guid ElectionGuid { get; set; }
        public System.Guid PersonGuid { get; set; }
        public Nullable<int> VoteCount { get; set; }
        public int Rank { get; set; }
        public string Section { get; set; }
        public Nullable<bool> CloseToPrev { get; set; }
        public Nullable<bool> CloseToNext { get; set; }
        public Nullable<bool> IsTied { get; set; }
        public Nullable<int> TieBreakGroup { get; set; }
        public Nullable<bool> TieBreakRequired { get; set; }
        public Nullable<int> TieBreakCount { get; set; }
        public Nullable<bool> IsTieResolved { get; set; }
        public Nullable<int> RankInExtra { get; set; }
        public Nullable<bool> ForceShowInOther { get; set; }
    }
}
