using System.Linq;
using TallyJ3.Code.Session;
using TallyJ3.Data.DbModel;

namespace TallyJ3.Data.Caching
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