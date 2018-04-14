using System;
using System.Linq;
using TallyJ3.Code.Session;
using TallyJ3.Data.DbModel;

namespace TallyJ3.Data.Caching
{
  public class BallotCacher : CacherBase<Ballot>
  {
    public override IQueryable<Ballot> MainQuery()
    {
      return CurrentDb.Ballot
        .Join(CurrentDb.Location.Where(l => l.ElectionGuid == CurrentElectionGuid), b => b.LocationGuid, l => l.LocationGuid, (b, l) => b);
    }

    protected override void ItemChanged()
    {
      new ResultSummaryCacher(CurrentDb).VoteOrPersonChanged();
    }

    private static object _lockObject;

    public BallotCacher(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    public BallotCacher() : base(UserSession.GetNewDbContext())
    {
    }

    public Ballot GetByComputerCode()
    {
      return AllForThisElection.FirstOrDefault(t => t.ComputerCode == UserSession.CurrentComputerCode);
    }

    protected override object LockCacheBaseObject
    {
      get
      {
        return _lockObject ?? (_lockObject = new object());
      }
    }
  }
}