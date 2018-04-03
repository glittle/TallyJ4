using System.Linq;
using TallyJ3.Code.Session;
using TallyJ3.Data.DbModel;

namespace TallyJ3.Data.Caching
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