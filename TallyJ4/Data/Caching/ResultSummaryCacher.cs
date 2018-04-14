using System.Data;
using System.Linq;
using TallyJ4.Code.Helper;
using TallyJ4.Code.Session;
using TallyJ4.Data.DbModel;

namespace TallyJ4.Data.Caching
{
    public class ResultSummaryCacher : CacherBase<ResultSummary>
    {
        public override IQueryable<ResultSummary> MainQuery()
        {
            return CurrentDb.ResultSummary.Where(p => p.ElectionGuid == CurrentElectionGuid);
        }

        public void VoteOrPersonChanged()
        {
            var results = AllForThisElection;
            if (results.Any(r => r.ResultType != ResultType.Manual))
            {
                //TODO does this do the SQL correctly??
                var query = CurrentDb.ResultSummary.Where(r => r.ResultType != ResultType.Manual);
                CurrentDb.ResultSummary.RemoveRange(query);

                results.RemoveAll(r => r.ResultType != ResultType.Manual);
                ReplaceEntireCache(results);
            }
        }

        private static object _lockObject;

        public ResultSummaryCacher(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public ResultSummaryCacher() : base(UserSession.GetNewDbContext())
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