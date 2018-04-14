namespace TallyJ4.Data.DbModel
{
    
    public partial class JoinElectionUser
    {
        public int Id { get; set; }
        public System.Guid ElectionGuid { get; set; }
        public System.Guid UserId { get; set; }
        public string Role { get; set; }
    
        public virtual ApplicationUser Users { get; set; }
    }
}
