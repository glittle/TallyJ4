using System;
using System.Linq;
using TallyJ3.Data;
using TallyJ3.Data.DbModel;

namespace TallyJ3.Models
{
  public class Savers : DataConnectedModel
  {
    //private readonly bool _isInTest;
    int _tempRowId = -1;

    //private readonly Func<Result, Result> _addResult;
    //private readonly Func<ResultSummary, ResultSummary> _addResultSummary;
    //private readonly Func<ResultTie, ResultTie> _addResultTie;
    //private ITallyJDbContext dbContext;

    //public Savers()
    //{
    //  _isInTest = false;
    //}

    //public Savers(bool isInTest)
    //{
    //  _isInTest = isInTest;
    //}

    //public Savers(IAnalyzerFakes fakes)
    //{
    //  _addResult = fakes.AddResult;
    //  _addResultSummary = fakes.AddResultSummary;
    //  _addResultTie = fakes.AddResultTie;
    //  _isInTest = true;
    //}

    public Savers(ApplicationDbContext dbContext)
    {
      SharedDbContext = dbContext;
    }

    public void PersonSaver(DbAction action, Person person)
    {
      switch (action)
      {
        case DbAction.Attach:
          //if (!_isInTest)
          {
            if (SharedDbContext.Person.Local.All(l => l.Id != person.Id))
            {
              SharedDbContext.Person.Attach(person);
            }
          }
          break;

        case DbAction.Save:
          //if (!_isInTest)
          {
            //new PersonCacher(SharedDbContext).UpdateItemAndSaveCache(person);
          }
          break;

        default:
          throw new ArgumentOutOfRangeException("action");
      }
    }

    public void VoteSaver(DbAction action, Vote vote)
    {
      switch (action)
      {
        case DbAction.Attach:
          //if (!_isInTest)
          {
            if (SharedDbContext.Vote.Local.All(l => l.Id != vote.Id))
            {
              SharedDbContext.Vote.Attach(vote);
            }
          }
          break;

        case DbAction.Save:
          //if (!_isInTest)
          {
            //new VoteCacher(SharedDbContext).UpdateItemAndSaveCache(vote);
          }
          break;

        default:
          throw new ArgumentOutOfRangeException("action");
      }
    }

    public void BallotSaver(DbAction action, Ballot ballot)
    {
      switch (action)
      {
        case DbAction.Attach:
          //if (!_isInTest)
          {
            if (SharedDbContext.Ballot.Local.All(l => l.Id != ballot.Id))
            {
              SharedDbContext.Ballot.Attach(ballot);
            }
          }
          break;

        case DbAction.Save:
          //if (!_isInTest)
          {
            //new BallotCacher(SharedDbContext).UpdateItemAndSaveCache(ballot);
          }
          break;

        default:
          throw new ArgumentOutOfRangeException("action");
      }
    }

    public void ResultSaver(DbAction action, Result result)
    {
      switch (action)
      {
        case DbAction.Add:
          //if (!_isInTest)
          {
            result.Id = _tempRowId--;
            SharedDbContext.Result.Add(result);
            //new ResultCacher(SharedDbContext).UpdateItemAndSaveCache(result);
          }
          //else
          //{
          //  _addResult(result);
          //}
          break;

        case DbAction.Attach:
          //if (!_isInTest)
          {
            if (SharedDbContext.Result.Local.All(r => r.Id != result.Id))
            {
              SharedDbContext.Result.Attach(result);
            }
          }
          break;

        case DbAction.Save:
          //if (!_isInTest)
          {
            //new ResultCacher(SharedDbContext).UpdateItemAndSaveCache(result);
          }
          break;

        case DbAction.AttachAndRemove:
          //if (!_isInTest)
          {
            //new ResultCacher(SharedDbContext).RemoveItemAndSaveCache(result);
            if (SharedDbContext.Result.Local.All(r => r.Id != result.Id))
            {
              SharedDbContext.Result.Attach(result);
            }
            SharedDbContext.Result.Remove(result);
          }
          break;

        default:
          throw new ArgumentOutOfRangeException("action");
      }
    }

    /// <Summary>Add this result to the datastore</Summary>
    //    protected void AddResultSummary(ResultSummary resultSummary)
    //    {
    //      ResultSummaries.Add(resultSummary);
    //      if (_addResultSummary != null)
    //      {
    //        _addResultSummary(resultSummary);
    //      }
    //      else
    //      {
    //        resultSummary.Id = tempRowId--;
    //        SharedDbContext.ResultSummary.Add(resultSummary);
    //        new ResultSummaryCacher(SharedDbContext).UpdateItemAndSaveCache(resultSummary);
    //      }
    //    }
    public void ResultSummarySaver(DbAction action, ResultSummary resultSummary)
    {
      switch (action)
      {
        case DbAction.Add:
          resultSummary.Id = _tempRowId--;
          //if (!_isInTest)
          {
            SharedDbContext.ResultSummary.Add(resultSummary);
            //new ResultSummaryCacher(SharedDbContext).UpdateItemAndSaveCache(resultSummary);
          }
          //else
          //{
          //  _addResultSummary(resultSummary);
          //}
          break;

        case DbAction.Attach:
          //if (!_isInTest)
          {
            if (SharedDbContext.ResultSummary.Local.All(r => r.Id != resultSummary.Id))
            {
              SharedDbContext.ResultSummary.Attach(resultSummary);
            }
          }
          break;

        case DbAction.Save:
          //if (!_isInTest)
          {
            //new ResultSummaryCacher(SharedDbContext).UpdateItemAndSaveCache(resultSummary);
          }
          break;

        case DbAction.AttachAndRemove:
          //if (!_isInTest)
          {
            //new ResultSummaryCacher(SharedDbContext).RemoveItemAndSaveCache(resultSummary);
            if (SharedDbContext.ResultSummary.Local.All(r => r.Id != resultSummary.Id))
            {
              SharedDbContext.ResultSummary.Attach(resultSummary);
            }
            SharedDbContext.ResultSummary.Remove(resultSummary);
          }
          break;

        default:
          throw new ArgumentOutOfRangeException("action");
      }
    }

    /// <Summary>Add this resultTie to the datastore</Summary>
    //protected Func<ResultTie, ResultTie> AddResultTie
    //{
    //  get { return _addResultTie ?? SharedDbContext.ResultTie.Add; }
    //}
    public void ResultTieSaver(DbAction action, ResultTie resultTie)
    {
      switch (action)
      {
        case DbAction.Add:
          //if (!_isInTest)
          {
            resultTie.Id = _tempRowId--;
            SharedDbContext.ResultTie.Add(resultTie);
            //new ResultTieCacher(SharedDbContext).UpdateItemAndSaveCache(resultTie);
          }
          //else
          //{
          //  _addResultTie(resultTie);
          //}
          break;

        case DbAction.Attach:
          //if (!_isInTest)
          {
            if (SharedDbContext.ResultTie.Local.All(r => r.Id != resultTie.Id))
            {
              SharedDbContext.ResultTie.Attach(resultTie);
            }
          }
          break;

        case DbAction.Save:
          //if (!_isInTest)
          {
            //new ResultTieCacher(SharedDbContext).UpdateItemAndSaveCache(resultTie);
          }
          break;

        case DbAction.AttachAndRemove:
          //if (!_isInTest && resultTie.Id > 0)
          if (resultTie.Id > 0)
          {
            //new ResultTieCacher(SharedDbContext).RemoveItemAndSaveCache(resultTie);
            if (SharedDbContext.ResultTie.Local.All(r => r.Id != resultTie.Id))
            {
              SharedDbContext.ResultTie.Attach(resultTie);
            }
            SharedDbContext.ResultTie.Remove(resultTie);
          }
          break;

        default:
          throw new ArgumentOutOfRangeException("action");
      }
    }
  }
}