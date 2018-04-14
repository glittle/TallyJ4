using System;

namespace TallyJ4.Data.DbModel
{
  [Serializable]
  public partial class Location : IIndexedForCaching
    {
        public int Id { get; set; }
        public System.Guid ElectionGuid { get; set; }
        public System.Guid LocationGuid { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
        public string TallyStatus { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<int> BallotsCollected { get; set; }
    }
}