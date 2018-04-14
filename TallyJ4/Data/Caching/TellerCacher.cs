using System.Linq;
using TallyJ4.Code.Session;
using TallyJ4.Data.DbModel;

namespace TallyJ4.Data.Caching
{
  public class TellerCacher : CacherBase<Teller>
  {
    public override IQueryable<Teller> MainQuery()
    {
      return CurrentDb.Teller.Where(p => p.ElectionGuid == CurrentElectionGuid);
    }

    private static object _lockObject;

    public TellerCacher(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    public TellerCacher() : base(UserSession.GetNewDbContext())
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