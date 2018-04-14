using System.Linq;
using TallyJ4.Code.Session;
using TallyJ4.Data.DbModel;

namespace TallyJ4.Data.Caching
{
    public class ResultTieCacher : CacherBase<ResultTie>
  {
    public override IQueryable<ResultTie> MainQuery()
    {
      return CurrentDb.ResultTie.Where(p => p.ElectionGuid == CurrentElectionGuid);
    }

    private static object _lockObject;

    public ResultTieCacher(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    public ResultTieCacher() : base(UserSession.GetNewDbContext())
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