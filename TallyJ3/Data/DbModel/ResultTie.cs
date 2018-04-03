using System;
using System.Collections.Generic;
namespace TallyJ3.Data.DbModel
{
    public partial class ResultTie : IIndexedForCaching
    {
        public int Id { get; set; }
        public System.Guid ElectionGuid { get; set; }
        public int TieBreakGroup { get; set; }
        public Nullable<bool> TieBreakRequired { get; set; }
        public int NumToElect { get; set; }
        public int NumInTie { get; set; }
        public Nullable<bool> IsResolved { get; set; }
    }
}
