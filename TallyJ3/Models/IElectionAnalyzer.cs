using System.Collections.Generic;
using TallyJ3.Data.DbModel;

namespace TallyJ3.Models
{
    public interface IElectionAnalyzer
    {
        /// <Summary>Current Results records</Summary>
        List<Result> Results { get; }

        List<ResultTie> ResultTies { get; }

        List<ResultSummary> ResultSummaries { get; }


        /// <Summary>Current Results records</Summary>
        ResultSummary ResultSummaryFinal { get; }

        /// <Summary>Current VoteInfo records</Summary>
        List<VoteInfo> VoteInfos { get; }

        /// <Summary>Indicate if the results are available, or need to be generated</Summary>
        bool IsResultAvailable { get; }

        List<Ballot> Ballots { get; }

        void AnalyzeEverything();

        void PrepareResultSummaries();

        void FinalizeSummaries();

        //TODO
        //IStatusUpdateHub AnalyzeHub { get; }

    }
}