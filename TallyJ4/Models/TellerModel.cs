using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using TallyJ4.Code.Session;
using TallyJ4.Data.Caching;
using TallyJ4.Extensions;
using TallyJ4.Data.DbModel;

namespace TallyJ4.Models
{
  public class TellerModel : DataConnectedModel
  {
    public object GrantAccessToGuestTeller(Guid electionGuid, string codeToTry, Guid oldComputerGuid)
    {
      var electionModel = new ElectionModel();

      var passcode = new PublicElectionLister().GetPasscodeIfAvailable(electionGuid);
      if (passcode == null)
      {
        return new
        {
          error = "Sorry, unknown election id"
        }.AsJsonResult();
      }
      if (passcode != codeToTry)
      {
        return new
        {
          error = "Sorry, invalid code entered"
        }.AsJsonResult();
      }

      if (!UserSession.IsLoggedIn)
      {
        var fakeUserName = Startup.HttpContext.Session.Id.Substring(0, 5) + Guid.NewGuid().ToString().Substring(0, 5);
        //TODO
        //FormsAuthentication.SetAuthCookie(fakeUserName, true);
        UserSession.IsGuestTeller = true;
      }

      electionModel.JoinIntoElection(electionGuid, oldComputerGuid);

      return new
      {
        loggedIn = true,
        compGuid = UserSession.CurrentComputer.ComputerGuid
      };
    }

    public object ChooseTeller(int num, int tellerId, string newName)
    {
      var helper = new TellerHelper();

      var db = GetNewDbContext();

      var tellerCacher = new TellerCacher(db);
      var computerCacher = new ComputerCacher();

      var currentComputer = UserSession.CurrentComputer;

      if (tellerId == 0)
      {
        UserSession.SetCurrentTeller(num, null);

        switch (num)
        {
          case 1:
            currentComputer.Teller1 = null;
            break;
          case 2:
            currentComputer.Teller2 = null;
            break;
        }

        computerCacher.UpdateComputer(currentComputer);

        return new { Saved = true };
      }

      Teller teller;

      if (tellerId == -1)
      {
        // add new
        // check for existing
        teller =
          tellerCacher.AllForThisElection.FirstOrDefault(t => t.Name.Equals(newName, StringComparison.OrdinalIgnoreCase));
        if (teller == null)
        {
          // add the new one
          teller = new Teller
          {
            ElectionGuid = UserSession.CurrentElectionGuid,
            Name = newName,
            UsingComputerCode = UserSession.CurrentComputerCode,
          };
          db.Teller.Add(teller);
          db.SaveChanges();
          tellerCacher.UpdateItemAndSaveCache(teller);
        }
      }
      else
      {
        // using existing
        teller = tellerCacher.GetById(tellerId);
        if (teller == null)
        {
          return new { Saved = false };
        }
      }

      switch (num)
      {
        case 1:
          currentComputer.Teller1 = teller.Name;
          break;
        case 2:
          currentComputer.Teller2 = teller.Name;
          break;
      }
      db.SaveChanges();
      computerCacher.UpdateComputer(currentComputer);

      UserSession.SetCurrentTeller(num, teller.Name);

      return new
      {
        Saved = true,
        Selected = teller.Id,
        TellerList = helper.GetTellerOptions(num)
      };

    }

    //    public static string GetTellerNames(Guid? tellerGuid1, Guid? tellerGuid2)
    //    {
    //      var tellers = new TellerCacher(Db).AllForThisElection;
    //
    //      var tellersOnThisComputer = new List<Teller>
    //      {
    //        tellers.FirstOrDefault(t => t.TellerGuid == tellerGuid1),
    //        tellers.FirstOrDefault(t => t.TellerGuid == tellerGuid2)
    //      };
    //      return tellersOnThisComputer.Select(t => t == null ? "" : t.Name).JoinedAsString(", ", true);
    //    }

    public static string GetTellerNames(string teller1, string teller2)
    {
      var tellersOnThisComputer = new List<string>
      {
        teller1,
        teller2
      };
      return tellersOnThisComputer.JoinedAsString(", ", true);
    }


    public object DeleteTeller(int tellerId)
    {
      var db = GetNewDbContext();
      var thisTeller = new TellerCacher(db).GetById(tellerId);

      if (thisTeller == null)
      {
        return new { Deleted = false, Error = "Not found" };
      }

      try
      {
        db.Teller.Attach(thisTeller);
        db.Teller.Remove(thisTeller);
        db.SaveChanges();

        new TellerCacher(db).RemoveItemAndSaveCache(thisTeller);
      }
      catch (Exception ex)
      {
        return new { Deleted = false, Error = ex.Message };
      }

      return new { Deleted = true };
    }
  }
}