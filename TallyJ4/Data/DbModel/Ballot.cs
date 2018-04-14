namespace TallyJ3.Data.DbModel
{
    
    public partial class Ballot : IIndexedForCaching
    {
        public int Id { get; set; }
        public System.Guid LocationGuid { get; set; }
        public System.Guid BallotGuid { get; set; }
        public string StatusCode { get; set; }
        public string ComputerCode { get; set; }
        public int BallotNumAtComputer { get; set; }
        public string C_BallotCode { get; set; }
        public string Teller1 { get; set; }
        public string Teller2 { get; set; }
        public byte[] C_RowVersion { get; set; }
    }
}
