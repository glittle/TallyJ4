using System;

namespace TallyJ3.Data.DbModel
{
    
    public partial class Teller : IIndexedForCaching
    {
        public int Id { get; set; }
        public System.Guid ElectionGuid { get; set; }
        public string Name { get; set; }
        public string UsingComputerCode { get; set; }
        public Nullable<bool> IsHeadTeller { get; set; }
        public byte[] C_RowVersion { get; set; }
    }
}
