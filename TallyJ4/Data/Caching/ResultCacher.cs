using System.Linq;
using TallyJ4.Code.Session;
using TallyJ4.Data.DbModel;

namespace TallyJ4.Data.Caching
{
    public class ResultCacher : CacherBase<Result>
    {
        public override IQueryable<Result> MainQuery()
        {
            return CurrentDb.Result.Where(p => p.ElectionGuid == CurrentElectionGuid);
        }

        private static object _lockObject;

        public ResultCacher(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public ResultCacher() : base(UserSession.GetNewDbContext())
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