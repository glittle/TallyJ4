using System.Linq;
using TallyJ4.Code.Session;
using TallyJ4.Data;
using TallyJ4.Data.Caching;
using TallyJ4.Data.DbModel;

namespace TallyJ4.Data.Caching
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