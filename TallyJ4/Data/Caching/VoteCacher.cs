using System.Linq;
using TallyJ4.Code.Session;
using TallyJ4.Data.DbModel;

namespace TallyJ4.Data.Caching
{
    public class VoteCacher : CacherBase<Vote>
  {
    public override IQueryable<Vote> MainQuery()
    {
      return CurrentDb.Vote
        .Join(CurrentDb.Ballot, v => v.BallotGuid, b => b.BallotGuid, (v, b) => new { v, b })
        .Join(CurrentDb.Location.Where(l => l.ElectionGuid == CurrentElectionGuid), g => g.b.LocationGuid,
          l => l.LocationGuid, (g, l) => g.v);
    }

    protected override void ItemChanged()
    {
      new ResultSummaryCacher(CurrentDb).VoteOrPersonChanged();
    }

    private static object _lockObject;

    public VoteCacher(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public VoteCacher() : base(UserSession.GetNewDbContext())
    {
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