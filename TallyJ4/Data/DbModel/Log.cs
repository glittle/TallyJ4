using System;

namespace TallyJ4.Data.DbModel
{
    
    public partial class Log
    {
        public int Id { get; set; }
        public System.DateTime AsOf { get; set; }
        public System.Guid ElectionGuid { get; set; }
        public Nullable<System.Guid> LocationGuid { get; set; }
        public string ComputerCode { get; set; }
        public string Details { get; set; }
    }
}
