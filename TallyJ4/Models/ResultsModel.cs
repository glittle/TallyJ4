using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TallyJ3.Code.Enumerations;
using TallyJ3.Code.Helper;
using TallyJ3.Code.Helpers;
using TallyJ3.Code.Hubs;
using TallyJ3.Code.Misc;
using TallyJ3.Code.Session;
using TallyJ3.Data.Caching;
using TallyJ3.Data.DbModel;
using TallyJ3.Extensions;

namespace TallyJ3.Models
{
    public class ResultsModel : DataConnectedModel
    {
        //TODO
        private readonly IElectionAnalyzer _analyzer;
        private Election _election;

        //public ResultsModel(Election election = null, IStatusUpdateHub hub = null)
        //{
        //    _election = election ?? UserSession.CurrentElection;

        //    _analyzer = CurrentElection.IsSingleNameElection
        //      ? new ElectionAnalyzerSingleName(CurrentElection, hub) as IElectionAnalyzer
        //      : new ElectionAnalyzerNormal(CurrentElection, hub);
        //}

        private Election CurrentElection
        {
            get { return _election; }
        }

        public JsonResult FinalResultsJson
        {
            get
            {
                _analyzer.PrepareResultSummaries();

                var resultSummaryFinal = _analyzer.ResultSummaryFinal;

                if (!resultSummaryFinal.UseOnReports.AsBoolean())
                {
                    // don't show any details if review is needed
                    return new
                    {
                        StatusText = "Analysis not complete",
                        Status = "Errors"
                    }.AsJsonResult();
                }

                //currentElection.ShowFullReport.AsBool() ? 99999 : 
                var numToShow = CurrentElection.NumberToElect.AsInt();
                var numExtra = CurrentElection.NumberExtra.AsInt();
                //                var numForChart = (numToShow + numExtra) * 2;

                var reportVotes =
                  new ResultCacher(SharedDbContext).AllForThisElection
                    .Join(new PersonCacher(SharedDbContext).AllForThisElection, r => r.PersonGuid, p => p.PersonGuid,
                      (r, p) => new { r, PersonName = p.FullNameFL })
                    .OrderBy(g => g.r.Rank)
                    .Take(numToShow + numExtra);

                //var chartVotes =
                //  Db.vResultInfoes.Where(ri => ri.ElectionGuid == CurrentElection.ElectionGuid).OrderBy(ri => ri.Rank)
                //    .Select(ri => new
                //                    {
                //                      ri.Rank,
                //                      ri.VoteCount
                //                    })
                //    .Take(numForChart);

                var tallyStatus = CurrentElection.TallyStatus;

                if (tallyStatus != ElectionTallyStatusEnum.Finalized)
                {
                    return new
                    {
                        Status = tallyStatus,
                        StatusText = ElectionTallyStatusEnum.TextFor(tallyStatus)
                    }.AsJsonResult();
                }

                return new
                {
                    ReportVotes =
                    reportVotes.Select(g => new
                    {
                        g.PersonName,
                        g.r.VoteCount,
                        TieBreakCount = g.r.IsTied.AsBoolean() ? g.r.TieBreakCount : null,
                        g.r.Section
                    }),
                    NumBallots = resultSummaryFinal.NumBallotsWithManual,
                    resultSummaryFinal.TotalVotes,
                    TotalInvalidVotes = resultSummaryFinal.SpoiledVotes,
                    TotalInvalidBallots = resultSummaryFinal.SpoiledBallots,
                    resultSummaryFinal.NumEligibleToVote,
                    EnvelopesMailedIn = resultSummaryFinal.MailedInBallots,
                    EnvelopesInPerson = resultSummaryFinal.InPersonBallots,
                    EnvelopesDroppedOff = resultSummaryFinal.DroppedOffBallots,
                    EnvelopesCalledIn = resultSummaryFinal.CalledInBallots,
                    resultSummaryFinal.NumVoters,
                    Participation = resultSummaryFinal.PercentParticipation,
                    Status = tallyStatus,
                    StatusText = ElectionTallyStatusEnum.TextFor(tallyStatus)
                }.AsJsonResult();
            }
        }

        public JsonResult GetCurrentResultsJson()
        {
            return GetCurrentResults().AsJsonResult();
        }

        public object GetCurrentResults()
        {
            //var ready = _analyzer.IsResultAvailable;
            var dbContext = GetNewDbContext();

            try
            {
                var resultSummaryFinal = _analyzer.ResultSummaryFinal; // resultSummaries.SingleOrDefault(rs => rs.ResultType == ResultType.Final);


                // don't show any details if review is needed
                if (resultSummaryFinal.BallotsNeedingReview != 0)
                {
                    var locations = new LocationCacher(dbContext).AllForThisElection;
                    var multipleLocations = locations.Count() > 1;

                    //TODO
                    var needReview = _analyzer.VoteInfos // .Where(VoteAnalyzer.VoteNeedReview)
                      .Join(locations, vi => vi.LocationId, l => l.Id,
                        (vi, location) => new { vi, location })
                      .Select(x => new
                      {
                          x.vi.LocationId,
                          x.vi.BallotId,
                          Status =
                          x.vi.BallotStatusCode == "Review"
                            ? BallotStatusEnum.Review.DisplayText
                            : "Verification Needed",
                          Ballot = multipleLocations ?
                          string.Format("{0} ({1})", x.vi.C_BallotCode, x.location.Name) : x.vi.C_BallotCode
                      })
                      .Distinct()
                      .OrderBy(x => x.Ballot);

                    var needReview2 = _analyzer.Ballots.Where(b => b.StatusCode == BallotStatusEnum.Review)
                      .Join(locations, b => b.LocationGuid, l => l.LocationGuid,
                        (b, location) => new { b, location })
                      .Select(x => new
                      {
                          LocationId = x.location.Id,
                          BallotId = x.b.Id,
                          Status =
                          x.b.StatusCode == "Review"
                            ? BallotStatusEnum.Review.DisplayText
                            : "Verification Needed",
                          Ballot = multipleLocations ?
                          string.Format("{0} ({1})", x.b.C_BallotCode, x.location.Name) : x.b.C_BallotCode
                      });

                    return new
                    {
                        NeedReview = needReview.Concat(needReview2).Distinct(),
                        ResultsFinal = _analyzer.ResultSummaryFinal
                          .GetPropertiesExcept(null, new[] { "ElectionGuid" }),
                        ResultsCalc =
                        _analyzer.ResultSummaries.First(rs => rs.ResultType == ResultType.Calculated)
                          .GetPropertiesExcept(null, new[] { "ElectionGuid" }),
                        ResultsManual = (_analyzer.ResultSummaries.FirstOrDefault(rs => rs.ResultType == ResultType.Manual) ??
                         new ResultSummary()).GetPropertiesExcept(null, new[] { "ElectionGuid" }),

                        //ResultsFinal =
                        //  resultSummaries.First(rs => rs.ResultType == ResultType.Final)
                        //    .GetPropertiesExcept(null, new[] { "ElectionGuid" }),
                    };
                }

                // show vote totals

                var persons = new PersonCacher(dbContext).AllForThisElection;

                var vResultInfos =
                  // TODO 2012-01-21 Glen Little: Could return fewer columns for non-tied results
                  _analyzer.Results // new ResultCacher(Db).AllForThisElection
                    .OrderBy(r => r.Rank)
                    .ToList()
                    .Select(r => new
                    {
                        rid = r.Id,
                        r.CloseToNext,
                        r.CloseToPrev,
                        r.ForceShowInOther,
                        r.IsTied,
                        r.IsTieResolved,
                        PersonName = PersonNameFor(persons, r),
                        r.Rank,
                //ri.RankInExtra,
                r.Section,
                        r.TieBreakCount,
                        r.TieBreakGroup,
                        r.TieBreakRequired,
                        r.VoteCount
                    }).ToList();

                var ties = _analyzer.ResultTies //  new ResultTieCacher(Db).AllForThisElection
                  .OrderBy(rt => rt.TieBreakGroup)
                  .Select(rt => new
                  {
                      rt.TieBreakGroup,
                      rt.NumInTie,
                      rt.NumToElect,
                      rt.TieBreakRequired,
                      rt.IsResolved
                  }).ToList();

                //var spoiledVotesSummary = Db.vVoteInfoes.where



                return new
                {
                    Votes = vResultInfos,
                    UserSession.CurrentElectionStatus,
                    Ties = ties,
                    NumToElect = _election.NumberToElect,
                    NumExtra = _election.NumberExtra,
                    ShowCalledIn = _election.UseCallInButton,
                    ResultsManual =
                    (_analyzer.ResultSummaries.FirstOrDefault(rs => rs.ResultType == ResultType.Manual) ?? new ResultSummary())
                      .GetPropertiesExcept(null, new[] { "ElectionGuid" }),
                    ResultsCalc =
                    _analyzer.ResultSummaries.First(rs => rs.ResultType == ResultType.Calculated)
                      .GetPropertiesExcept(null, new[] { "ElectionGuid" }),
                    ResultsFinal =
                    _analyzer.ResultSummaries.First(rs => rs.ResultType == ResultType.Final)
                      .GetPropertiesExcept(null, new[] { "ElectionGuid" }),
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Interrupted = true,
                    Msg = ex.GetAllMsgs("; ") + "\n" + ex.StackTrace
                };
            }

        }

        private string PersonNameFor(IEnumerable<Person> persons, Result result)
        {
            var personGuid = result.PersonGuid;
            var person = persons.FirstOrDefault(p => p.PersonGuid == personGuid);
            return person == null ? "?" : person.FullNameFL;
        }

        public void GenerateResults()
        {
            _analyzer.AnalyzeEverything();
        }

        public object GetCurrentResultsIfAvailable()
        {
            return _analyzer.IsResultAvailable ? GetCurrentResults() : null;
        }

        //public bool HasTies()
        //{
        //  if (_analyzer.IsResultAvailable)
        //  {
        //    return _analyzer.ResultTies.Count > 0;
        //  }

        //  return false;
        //}

        public JsonResult GetReportData(string code)
        {
            var summary = new ResultSummaryCacher(GetNewDbContext()).AllForThisElection.SingleOrDefault(rs => rs.ResultType == ResultType.Final);

            var status = "ok";
            var currentElection = CurrentElection;
            var electionStatus = CurrentElection.TallyStatus;

            var readyForReports = summary != null && summary.UseOnReports.AsBoolean() && electionStatus == ElectionTallyStatusEnum.Finalized;

            var html = "";
            switch (code)
            {
                case "SimpleResults":
                    if (summary == null)
                    {
                        status = "Results not available. Please view 'Analyze' page first.";
                        electionStatus = currentElection.TallyStatus;
                    }
                    else
                    {
                        html = MvcViewRenderer.RenderRazorViewToString("~/Reports/Main.cshtml", summary);
                    }
                    break;

                case "": // not sure how this happens
                    return new { Status = "Unknown report" }.AsJsonResult();

                default:
                    html = MvcViewRenderer.RenderRazorViewToString("~/Reports/{0}.cshtml".FilledWith(code));
                    if (html.HasNoContent())
                    {
                        return new { Status = "Unknown report" }.AsJsonResult();
                    }
                    break;
            }

            return new
            {
                Html = html,
                Status = status,
                Ready = readyForReports,
                ElectionStatus = electionStatus,
                ElectionStatusText = ElectionTallyStatusEnum.TextFor(electionStatus)
            }.AsJsonResult();
        }

        public JsonResult SaveTieCounts(List<string> counts)
        {
            if (counts == null || counts.Count == 0)
            {
                return new
                {
                    Msg = "Nothing to Save"
                }.AsJsonResult();
            }
            // input like:   2_3,5_3,235_0
            var countItems = counts.Select(delegate (string s)
            {
                var parts = s.SplitWithString("_", StringSplitOptions.None);
                return new
                {
                    ResultId = parts[0].AsInt(),
                    Value = parts[1].AsInt()
                };
            }).ToList();

            var resultsIds = countItems.Select(ci => ci.ResultId).ToArray();

            var dbContext = GetNewDbContext();

            var resultCacher = new ResultCacher(dbContext);
            var results = resultCacher.AllForThisElection.Where(r => resultsIds.Contains(r.Id)).ToList();

            foreach (var result in results)
            {
                dbContext.Result.Attach(result);

                result.TieBreakCount = countItems.Single(ci => ci.ResultId == result.Id).Value;

                resultCacher.UpdateItemAndSaveCache(result);
            }

            dbContext.SaveChanges();

            return new
            {
                Saved = true
            }.AsJsonResult();
        }

        public JsonResult SaveManualResults(ResultSummary manualResultsFromBrowser)
        {
            var dbContext = GetNewDbContext();
            ResultSummary resultSummary = null;
            var resultSummaryCacher = new ResultSummaryCacher(dbContext);

            if (manualResultsFromBrowser.Id != 0)
            {
                resultSummary = resultSummaryCacher.AllForThisElection.FirstOrDefault(rs =>
                  rs.Id == manualResultsFromBrowser.Id
                  && rs.ResultType == ResultType.Manual);
            }
            if (resultSummary == null)
            {
                resultSummary = new ResultSummary
                {
                    ElectionGuid = UserSession.CurrentElectionGuid,
                    ResultType = ResultType.Manual
                };
                dbContext.ResultSummary.Add(resultSummary);
                dbContext.SaveChanges();

                resultSummaryCacher.UpdateItemAndSaveCache(resultSummary);
            }
            else
            {
                dbContext.ResultSummary.Attach(resultSummary);
            }

            var editableFields = new
            {
                resultSummary.BallotsNeedingReview,
                resultSummary.BallotsReceived,
                resultSummary.CalledInBallots,
                resultSummary.DroppedOffBallots,
                resultSummary.InPersonBallots,
                resultSummary.MailedInBallots,
                resultSummary.NumEligibleToVote,
                resultSummary.SpoiledManualBallots,
            }.GetAllPropertyInfos().Select(pi => pi.Name).ToArray();

            var changed = manualResultsFromBrowser.CopyPropertyValuesTo(resultSummary, editableFields);

            if (!changed)
            {
                return new
                {
                    Message = "No changes"
                }.AsJsonResult();
            }

            dbContext.SaveChanges();

            Startup.GetService<IAnalyzeHubHelper>().StatusUpdate("Starting Analysis from " + UserSession.CurrentComputerCode);

            resultSummaryCacher.UpdateItemAndSaveCache(resultSummary);

            _analyzer.AnalyzeEverything();
            //.PrepareResultSummaries();
            //_analyzer.FinalizeSummaries();

            var resultSummaries = _analyzer.ResultSummaries;

            return new
            {
                Saved = true,
                ResultsManual =
                (resultSummaries.FirstOrDefault(rs => rs.ResultType == ResultType.Manual) ??
                 new ResultSummary())
                  .GetPropertiesExcept(null, new[] { "ElectionGuid" }),
                ResultsFinal =
                resultSummaries.First(rs => rs.ResultType == ResultType.Final)
                  .GetPropertiesExcept(null, new[] { "ElectionGuid" }),
            }.AsJsonResult();
        }

        public object GetTies(int tieBreakGroup)
        {
            var db = GetNewDbContext();
            return new ResultCacher(db).AllForThisElection
              .Where(r => r.TieBreakGroup == tieBreakGroup)
              .Join(new PersonCacher(db).AllForThisElection, r => r.PersonGuid, p => p.PersonGuid, (r, p) => new { r, p })
              .Select(j => new
              {
                  name = j.p.FullNameFL,
                  isResolved = j.r.IsTieResolved.AsBoolean()
              })
              .OrderBy(x => x.name);
        }
        public List<ResultTie> GetUnresolvedTieBreakGroups()
        {
            var Db = GetNewDbContext();

            // was caching old info
            new ResultTieCacher(Db).DropThisCache();

            return _analyzer.ResultTies
              .Where(rt => !rt.IsResolved.AsBoolean())
              .OrderBy(rt => rt.TieBreakGroup)
              .ToList();
        }
    }
}