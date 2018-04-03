using System.Linq;
using TallyJ3.Code.Session;
using TallyJ3.Data.DbModel;

namespace TallyJ3.Data.Caching
{
    public class PersonCacher : CacherBase<Person>
  {
    public override IQueryable<Person> MainQuery()
    {
      return CurrentDb.Person.Where(p => p.ElectionGuid == CurrentElectionGuid);
    }
  
    protected override void ItemChanged()
    {
      new ResultSummaryCacher(CurrentDb).VoteOrPersonChanged();
    }

    private static object _lockObject;

    public PersonCacher(ApplicationDbContext dbContext) : base(dbContext) {
    }
    public PersonCacher() : base(UserSession.GetNewDbContext())
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