using System.Linq;
using TallyJ3.Code.Session;
using TallyJ3.Data;
using TallyJ3.Data.Caching;
using TallyJ3.Data.DbModel;

namespace TallyJ3.EF
{
  public class LocationCacher : CacherBase<Location>
  {
    public override IQueryable<Location> MainQuery()
    {
      return CurrentDb.Location.Where(p => p.ElectionGuid == UserSession.CurrentElectionGuid);
    }

    private static object _lockObject;

    public LocationCacher(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    public LocationCacher() : base(UserSession.GetNewDbContext())
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