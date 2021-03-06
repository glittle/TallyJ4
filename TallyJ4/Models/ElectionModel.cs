using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TallyJ4.Code.Enumerations;
using TallyJ4.Code.Helper;
using TallyJ4.Code.Hubs;
using TallyJ4.Code.Misc;
using TallyJ4.Code.Session;
using TallyJ4.Data.Caching;
using TallyJ4.Data.DbModel;
using TallyJ4.Extensions;

namespace TallyJ4.Models
{
    public class ElectionModel : DataConnectedModel
    {
        private static object LockObject = new object();

        public ElectionRules GetRules(string type, string mode)
        {
            var rules = new ElectionRules
            {
                Num = 0,
                Extra = 0,
                CanVote = "",
                CanReceive = "",
                IsSingleNameElection = false
            };


            switch (type)
            {
                case "LSA":
                    rules.CanVote = CanVoteOrReceive.All;
                    rules.CanVoteLocked = true;

                    rules.Extra = 0;
                    rules.ExtraLocked = true;

                    switch (mode)
                    {
                        case ElectionMode.Normal:
                            rules.Num = 9;
                            rules.NumLocked = true;
                            rules.CanReceive = CanVoteOrReceive.All;
                            break;
                        case ElectionMode.TieBreak:
                            rules.Num = 1;
                            rules.NumLocked = false;
                            rules.CanReceive = CanVoteOrReceive.NamedPeople;
                            break;
                        case ElectionMode.ByElection:
                            rules.Num = 1;
                            rules.NumLocked = false;
                            rules.CanReceive = CanVoteOrReceive.All;
                            break;
                    }
                    rules.CanReceiveLocked = true;

                    break;

                case "NSA":
                    rules.CanVote = CanVoteOrReceive.NamedPeople; // delegates
                    rules.CanVoteLocked = true;

                    rules.Extra = 0;
                    rules.ExtraLocked = true;

                    switch (mode)
                    {
                        case ElectionMode.Normal:
                            rules.Num = 9;
                            rules.NumLocked = true;
                            rules.CanReceive = CanVoteOrReceive.All;
                            break;
                        case ElectionMode.TieBreak:
                            rules.Num = 1;
                            rules.NumLocked = false;
                            rules.CanReceive = CanVoteOrReceive.NamedPeople;
                            break;
                        case ElectionMode.ByElection:
                            rules.Num = 1;
                            rules.NumLocked = false;
                            rules.CanReceive = CanVoteOrReceive.All;
                            break;
                    }

                    rules.CanReceiveLocked = true;

                    break;

                case "Con":
                    rules.CanVote = CanVoteOrReceive.All;
                    rules.CanVoteLocked = true;

                    switch (mode)
                    {
                        case ElectionMode.Normal:
                            rules.Num = 5;
                            rules.NumLocked = false;

                            rules.Extra = 3;
                            rules.ExtraLocked = false;

                            rules.CanReceive = CanVoteOrReceive.All;
                            break;

                        case ElectionMode.TieBreak:
                            rules.Num = 1;
                            rules.NumLocked = false;

                            rules.Extra = 0;
                            rules.ExtraLocked = true;

                            rules.CanReceive = CanVoteOrReceive.NamedPeople;
                            break;

                        case ElectionMode.ByElection:
                            throw new ApplicationException("Unit Conventions cannot have by-elections");
                    }
                    rules.CanReceiveLocked = true;
                    break;

                case "Reg":
                    rules.CanVote = CanVoteOrReceive.NamedPeople; // LSA members
                    rules.CanVoteLocked = false;

                    switch (mode)
                    {
                        case ElectionMode.Normal:
                            rules.Num = 9;
                            rules.NumLocked = false;

                            rules.Extra = 7;
                            rules.ExtraLocked = false;

                            rules.CanReceive = CanVoteOrReceive.All;
                            break;

                        case ElectionMode.TieBreak:
                            rules.Num = 1;
                            rules.NumLocked = false;

                            rules.Extra = 0;
                            rules.ExtraLocked = true;

                            rules.CanReceive = CanVoteOrReceive.NamedPeople;
                            break;

                        case ElectionMode.ByElection:
                            // Regional Councils often do not have by-elections, but some countries may allow it?

                            rules.Num = 1;
                            rules.NumLocked = false;

                            rules.Extra = 0;
                            rules.ExtraLocked = true;

                            rules.CanReceive = CanVoteOrReceive.All;
                            break;
                    }
                    rules.CanReceiveLocked = true;
                    break;

                case "Oth":
                    rules.CanVote = CanVoteOrReceive.All;

                    rules.CanVoteLocked = false;
                    rules.CanReceiveLocked = false;
                    rules.NumLocked = false;
                    rules.ExtraLocked = false;

                    switch (mode)
                    {
                        case ElectionMode.Normal:
                            rules.Num = 9;
                            rules.Extra = 0;
                            rules.CanReceive = CanVoteOrReceive.All;
                            break;

                        case ElectionMode.TieBreak:
                            rules.Num = 1;
                            rules.Extra = 0;
                            rules.CanReceive = CanVoteOrReceive.NamedPeople;
                            break;

                        case ElectionMode.ByElection:
                            rules.Num = 1;
                            rules.Extra = 0;
                            rules.CanReceive = CanVoteOrReceive.All;
                            break;
                    }
                    break;
            }

            return rules;
        }

        /// <summary>
        ///   Based on the 2 flags, get a default reason (may be null)
        /// </summary>
        /// <returns></returns>
        public IneligibleReasonEnum GetDefaultIneligibleReason()
        {
            return GetDefaultIneligibleReason(UserSession.CurrentElection);
        }

        /// <summary>
        ///   Based on the 2 flags, get a default reason (may be null)
        /// </summary>
        /// <returns></returns>
        public static IneligibleReasonEnum GetDefaultIneligibleReason(Election election)
        {
            if (election == null)
            {
                return null;
            }

            var canVote = election.CanVote == CanVoteOrReceive.All;
            var canReceiveVotes = election.CanReceive == CanVoteOrReceive.All;

            if (canVote && canReceiveVotes)
            {
                return null;
            }
            if (!canVote && !canReceiveVotes)
            {
                return IneligibleReasonEnum.Ineligible_Other;
            }
            if (!canVote)
            {
                return IneligibleReasonEnum.IneligiblePartial2_Not_a_Delegate;
            }
            return IneligibleReasonEnum.IneligiblePartial1_Not_in_TieBreak;
        }

        //    public Election GetFreshFromDb(Guid electionGuid)
        //    {
        //      return Election.ThisElectionCached;// Db.Election.FirstOrDefault(e => e.ElectionGuid == electionGuid);
        //    }
        /// <Summary>Gets directly from the database, not session. Stores in session.</Summary>
        /// <Summary>Saves changes to this election</Summary>
        public JsonResult SaveElection(Election electionFromBrowser)
        {
            var Db = GetNewDbContext();
            //      var election = Db.Election.SingleOrDefault(e => e.Id == electionFromBrowser.Id);
            var electionCacher = new ElectionCacher(Db);

            var election = UserSession.CurrentElection;
            Db.Election.Attach(election);

            var currentType = election.ElectionType;
            var currentMode = election.ElectionMode;
            var currentCan = election.CanVote;
            var currentReceive = election.CanReceive;
            var currentListed = election.ListForPublic;

            // List of fields to allow edit from setup page
            var editableFields = new
            {
                election.Name,
                election.DateOfElection,
                election.Convenor,
                election.ElectionType,
                election.ElectionMode,
                election.NumberToElect,
                election.NumberExtra,
                election.CanVote,
                election.CanReceive,
                election.ListForPublic,
                election.ShowAsTest,
                election.ElectionPasscode,
                election.UseCallInButton,
                election.HidePreBallotPages,
                election.MaskVotingMethod,
                election.BallotProcess,
                election.EnvNumMode,
                election.T24,
            }.GetAllPropertyInfos().Select(pi => pi.Name).ToArray();

            if (!currentListed.AsBoolean() && election.ListForPublic.AsBoolean())
            {
                // just turned on
                election.ListedForPublicAsOf = DateTime.Now;
            }

            var changed = electionFromBrowser.CopyPropertyValuesTo(election, editableFields);

            if (changed)
            {
                Db.SaveChanges();

                electionCacher.UpdateItemAndSaveCache(election);

                Startup.GetService<IPublicHubHelper>().TellPublicAboutVisibleElections(); // in case the name, or ListForPublic, etc. has changed
            }

            if (currentMode != election.ElectionMode
                || currentType != election.ElectionType
                || currentCan != election.CanVote
                || currentReceive != election.CanReceive
              )
            {
                // reset flags
                new PeopleModel().SetInvolvementFlagsToDefault();

                // update analysis
                new ResultsModel().GenerateResults();
            }

            var displayName = UserSession.CurrentElectionDisplayNameAndInfo;

            return new
            {
                Status = "Saved",
                Election = election,
                displayName
            }.AsJsonResult();
        }

        public bool JoinIntoElection(int wantedElectionId, Guid oldComputerGuid)
        {
            var electionGuid = SharedDbContext.Election.Where(e => e.Id == wantedElectionId).Select(e => e.ElectionGuid).FirstOrDefault();
            if (electionGuid == Guid.Empty)
            {
                return false;
            }
            return JoinIntoElection(electionGuid, oldComputerGuid);
        }
        public bool JoinIntoElection(Guid wantedElectionGuid, Guid oldComputerGuid)
        {
            // don't use cache, go directly to database - cache is tied to current election
            var exists = SharedDbContext.Election.Any(e => e.ElectionGuid == wantedElectionGuid);
            if (!exists)
            {
                return false;
            }

            if (UserSession.CurrentElectionGuid == wantedElectionGuid)
            {
                return true;
            }


            // switch this the whole environment to use this election
            UserSession.LeaveElection(true);

            // move into new election
            UserSession.CurrentElectionGuid = wantedElectionGuid;

            // assign new computer code
            var computerModel = new ComputerModel();
            computerModel.GetComputerForMe(oldComputerGuid);

            string message;
            if (UserSession.IsGuestTeller)
            {
                message = "Guest teller joined into Election";
            }
            else
            {
                message = "Teller (" + UserSession.MemberName + ") switched into Election";

                Startup.GetService<IPublicHubHelper>().TellPublicAboutVisibleElections();

                UpgradeOldData();
            }

            new LogHelper().Add("{0} (Comp {1})".FilledWith(message, UserSession.CurrentComputerCode), true);

            return true;
        }

        private void UpgradeOldData()
        {
            var Db = GetNewDbContext();
            var personCacher = new PersonCacher(Db);
            var testInfo = personCacher.AllForThisElection.Select(p => new { p.CombinedInfo }).FirstOrDefault();

            if (testInfo == null)
            {
                return;
            }

            if (testInfo.CombinedInfo.HasContent() && !testInfo.CombinedInfo.Contains("^"))
            {
                return;
            }

            // fix all data
            var voteCacher = new VoteCacher(Db);

            var people = personCacher.AllForThisElection;
            var votes = voteCacher.AllForThisElection;

            var peopleModel = new PeopleModel();
            var saveNeeded = false;

            foreach (var person in people)
            {
                AutoFix(person, votes, peopleModel, ref saveNeeded);
            }

            if (saveNeeded)
            {
                Db.SaveChanges();

                new LogHelper().Add("Updated person combined infos");
            }

            personCacher.DropThisCache();
            voteCacher.DropThisCache();
        }

        public void AutoFix(Person person, List<Vote> voteList, PeopleModel peopleModel, ref bool saveNeeded)
        {
            var oldCombined = person.CombinedInfo;

            peopleModel.SetCombinedInfos(person);

            if (person.CombinedInfo == oldCombined)
            {
                //didn't need to fix it
                return;
            }

            saveNeeded = true;

            foreach (var vote in voteList.Where(v => v.PersonGuid == person.PersonGuid))
            {
                vote.PersonCombinedInfo = person.CombinedInfo;
            }
        }


        //public JsonResult Copy(Guid guidOfElectionToCopy)
        //{
        //    if (UserSession.IsGuestTeller)
        //    {
        //        return new
        //                 {
        //                     Success = false,
        //                     Message = "Not authorized"
        //                 }.AsJsonResult();
        //    }

        //    var election = Db.Election.SingleOrDefault(e => e.ElectionGuid == guidOfElectionToCopy);
        //    if (election == null)
        //    {
        //        return new
        //                 {
        //                     Success = false,
        //                     Message = "Not found"
        //                 }.AsJsonResult();
        //    }

        //    // copy in SQL
        //    var result = Db.CloneElection(election.ElectionGuid, UserSession.LoginId).SingleOrDefault();
        //    if (result == null)
        //    {
        //        return new
        //                 {
        //                     Success = false,
        //                     Message = "Unable to copy"
        //                 }.AsJsonResult();
        //    }
        //    if (!result.Success.AsBoolean())
        //    {
        //        return new
        //                 {
        //                     Success = false,
        //                     Message = "Sorry: " + result.Message
        //                 }.AsJsonResult();
        //    }
        //    election = Db.Election.SingleOrDefault(e => e.ElectionGuid == result.NewElectionGuid);
        //    if (election == null)
        //    {
        //        return new
        //                 {
        //                     Success = false,
        //                     Message = "New election not found"
        //                 }.AsJsonResult();
        //    }
        //    UserSession.CurrentElection = election;
        //    return new
        //             {
        //                 Success = true,
        //                 election.ElectionGuid
        //             }.AsJsonResult();
        //}

        public JsonResult Create()
        {
            if (UserSession.IsGuestTeller)
            {
                return new
                {
                    Success = false,
                    Message = "Not authorized"
                }.AsJsonResult();
            }

            var Db = GetNewDbContext();

            // create an election for this ID
            // create a default Location
            // assign all of these to this person and computer

            UserSession.ResetWhenSwitchingElections();

            var election = new Election
            {
                Convenor = "[Convener]", // correct spelling is Convener. DB field name is wrong.
                ElectionGuid = Guid.NewGuid(),
                Name = "[New Election]",
                ElectionType = "LSA",
                ElectionMode = ElectionMode.Normal,
                TallyStatus = ElectionTallyStatusEnum.NotStarted,
                NumberToElect = 9,
                NumberExtra = 0,
                CanVote = CanVoteOrReceive.All,
                CanReceive = CanVoteOrReceive.All
            };

            Db.Election.Add(election);
            Db.SaveChanges();

            UserSession.CurrentElectionGuid = election.ElectionGuid;

            //      new ElectionStatusSharer().SetStateFor(election);

            var join = new JoinElectionUser
            {
                ElectionGuid = election.ElectionGuid,
                UserId = UserSession.UserGuid
            };
            Db.JoinElectionUser.Add(join);


            var mainLocation = new Location
            {
                Name = "Main Location",
                LocationGuid = Guid.NewGuid(),
                ElectionGuid = election.ElectionGuid,
                SortOrder = 1
            };
            Db.Location.Add(mainLocation);
            Db.SaveChanges();
            new LocationCacher(Db).UpdateItemAndSaveCache(mainLocation);

            //      var computerModel = new ComputerModel();
            //      computerModel.CreateComputerForMe();

            return new
            {
                Success = true
            }.AsJsonResult();
        }

        public void SetTallyStatus(string status)
        {
            if (UserSession.IsGuestTeller)
            {
                return;
            }
            var Db = GetNewDbContext();

            var electionCacher = new ElectionCacher(Db);
            var election = UserSession.CurrentElection;

            if (election.TallyStatus != status)
            {
                Db.Election.Attach(election);

                election.TallyStatus = status;

                Db.SaveChanges();

                electionCacher.UpdateItemAndSaveCache(election);

                //new ElectionStatusSharer().SetStateFor(election);
                //        var menuHelper = new MenuHelper(controller.Url);
                var info = new
                {
                    //          QuickLinks = menuHelper.QuickLinks(),
                    //          Name = UserSession.CurrentElectionStatusName,
                    StateName = UserSession.CurrentElectionStatus,
                };
                //
                //        // should always be true... but usage could change in future
                //        var currentIsKnown = UserSession.IsKnownTeller;
                //        UserSession.IsKnownTeller = false;
                ////        menuHelper = new MenuHelper(controller.Url);
                //        var infoForGuest = new
                //        {
                ////          QuickLinks = menuHelper.QuickLinks(),
                ////          QuickSelector = menuHelper.StateSelectorItems().ToString(),
                ////          Name = UserSession.CurrentElectionStatusName,
                //          State = UserSession.CurrentElectionStatus,
                //        };
                //        UserSession.IsKnownTeller = currentIsKnown;

                Startup.GetService<IMainHubHelper>().StatusChanged(info, info);
            }
        }

        //    public IEnumerable<Election> VisibleElections()
        //    {
        //      // this is first hit on the database on the home page... need special logging
        //      try
        //      {
        //        var electionsWithCode =
        //          Db.Election.Where(e => e.ElectionPasscode != null && e.ElectionPasscode != "").ToList();
        //        return
        //          electionsWithCode.Where(
        //            e => e.ListForPublic.AsBoolean() && DateTime.Now - e.ListedForPublicAsOf <= 5.minutes());
        //      }
        //      catch (Exception e)
        //      {
        //        var logger = LogManager.GetCurrentClassLogger();
        //        logger.ErrorException("Reading VisibleElections", e);
        //
        //        return new List<Election>();
        //      }
        //    }

        public JsonResult SetTallyStatusJson(string status)
        {
            var Db = GetNewDbContext();

            var summary = new ResultSummaryCacher(Db).AllForThisElection.SingleOrDefault(rs => rs.ResultType == ResultType.Final);
            var readyForReports = summary != null && summary.UseOnReports.AsBoolean();
            if (status == ElectionTallyStatusEnum.Finalized && !readyForReports)
            {
                return new
                {
                    Message = "Cannot set to \"Finalized\" until Analysis is completed successfully."
                }.AsJsonResult();
            }

            SetTallyStatus(status);

            new LogHelper().Add("Status changed to " + status, true);

            return new
            {
                // QuickLinks = new MenuHelper(controller.Url).QuickLinks(),
                StateName = UserSession.CurrentElectionStatus
            }.AsJsonResult();
        }

        internal int GetNextEnvelopeNumber()
        {
            // create a new env number - Jan 2018 - create number for each ballot. it may be displayed or not

            // do we need a transaction here to ensure no duplicates are made?

            int nextNum;
            var Db = GetNewDbContext();

            lock (LockObject)
            {
                var election = UserSession.CurrentElection;
                Db.Election.Attach(election);

                nextNum = election.LastEnvNum.AsInt() + 1;
                election.LastEnvNum = nextNum;

                new ElectionCacher(Db).UpdateItemAndSaveCache(election);
            }

            Db.SaveChanges(); // save immediately, may include person saving

            return nextNum;
        }

        public JsonResult UpdateElectionShowAllJson(bool showFullReport)
        {
            var Db = SharedDbContext;
            var electionCacher = new ElectionCacher(Db);

            var election = UserSession.CurrentElection;
            if (election.ShowFullReport != showFullReport)
            {
                Db.Election.Attach(election);

                election.ShowFullReport = showFullReport;

                Db.SaveChanges();

                electionCacher.UpdateItemAndSaveCache(election);
            }

            return new { Saved = true }.AsJsonResult();
        }


        public JsonResult UpdateListOnPageJson(bool listOnPage)
        {
            if (UserSession.IsKnownTeller)
            {
                var Db = GetNewDbContext();

                var electionCacher = new ElectionCacher(Db);

                var election = UserSession.CurrentElection;
                Db.Election.Attach(election);

                election.ListForPublic = listOnPage;
                election.ListedForPublicAsOf = listOnPage ? (DateTime?)DateTime.Now : null;

                Db.SaveChanges();

                electionCacher.UpdateItemAndSaveCache(election);

                Startup.GetService<IPublicHubHelper>().TellPublicAboutVisibleElections();

                return new { Saved = true }.AsJsonResult();
            }

            return new { Saved = false }.AsJsonResult();
        }

        public bool GuestsAllowed()
        {
            var election = UserSession.CurrentElection;
            return election != null && election.CanBeAvailableForGuestTellers;
        }

        /// <summary>
        ///   Closes the election, logging out all guests.
        ///   If another known teller is still logged in, they will need to access the server to open the election again.
        /// </summary>
        public void CloseElection()
        {
            var election = UserSession.CurrentElection;

            if (election != null)
            {
                var Db = GetNewDbContext();

                if (Db.Election.Local.All(e => e.ElectionGuid != election.ElectionGuid))
                {
                    Db.Election.Attach(election);
                }

                election.ListedForPublicAsOf = null;

                Db.SaveChanges();

                Startup.GetService<IMainHubHelper>().CloseOutGuestTellers();

                new BallotCacher().DropThisCache();
                //new ComputerCacher().DropThisCache();
                new ElectionCacher().DropThisCache();
                new LocationCacher().DropThisCache();
                new PersonCacher().DropThisCache();
                new ResultCacher().DropThisCache();
                new ResultSummaryCacher().DropThisCache();
                new ResultTieCacher().DropThisCache();
                new TellerCacher().DropThisCache();
                new VoteCacher().DropThisCache();
            }

            Startup.GetService<IPublicHubHelper>().TellPublicAboutVisibleElections();
        }

        //    public bool ProcessPulse()
        //    {
        //      if (!UserSession.CurrentElectionGuid.HasContent())
        //      {
        //        return false;
        //      }
        //
        //
        //      //      var sharer = new ElectionStatusSharer();
        //      //      var sharedState = sharer.GetStateFor(UserSession.CurrentElectionGuid);
        //      //      var someoneElseChangedTheStatus = sharedState != election.TallyStatus;
        //      //      if (someoneElseChangedTheStatus)
        //      //      {
        //      //        //election = GetFreshFromDb(election.ElectionGuid);
        //      //        sharer.SetStateFor(election);
        //      //      }
        //
        //      UpdateListedForPublicTime();
        //
        //      //      return someoneElseChangedTheStatus;
        //      return true;
        //    }
        /// <Summary>Do any processing for the election</Summary>
        /// <returns> True if the status changed </returns>
        /// <summary>
        ///   Should be called whenever the known teller has contacted us
        /// </summary>
        //    public static void UpdateListedForPublicTime()
        //    {
        //      if (!UserSession.IsKnownTeller)
        //      {
        //        return;
        //      }
        //
        //      var election = UserSession.CurrentElection;
        //      if (election == null || !election.ListForPublic.AsBoolean())
        //      {
        //        return;
        //      }
        //
        //      // don't bother saving this to the database
        //      var now = DateTime.Now;
        //
        //      if (now - election.ListedForPublicAsOf < 1.minutes())
        //      {
        //        // don't need to update in less than a minute
        //        return;
        //      }
        //
        //      election.ListedForPublicAsOf = now;
        //
        //      var electionCacher = new ElectionCacher(Db);
        //      electionCacher.UpdateItemAndSaveCache(election);
        //    }

        //public void UpdateElectionWhenComputerFreshnessChanges(List<Computer> computers = null)
        //{
        //  var currentElection = UserSession.CurrentElection;
        //  if (currentElection == null)
        //  {
        //    return;
        //  }

        //  var lastContactOfTeller = (computers ?? new ComputerCacher().AllForThisElection)
        //    .Where(c => c.AuthLevel == "Known")
        //    .Max(c => c.LastContact);

        //  if (lastContactOfTeller != null &&
        //      (currentElection.ListedForPublicAsOf == null
        //       ||
        //       Math.Abs((DateTime.Now - lastContactOfTeller.Value).TotalMinutes) >
        //       5.minutes().TotalMinutes))
        //  {
        //    currentElection.ListedForPublicAsOf = lastContactOfTeller;
        //    new ElectionCacher(Db).UpdateItemAndSaveCache(currentElection);
        //  }

        //  //new PublicElectionLister().UpdateThisElectionInList();
        //  new PublicHub().TellPublicAboutVisibleElections();

        //}



        public static class CanVoteOrReceive
        {
            public const string All = "A";
            public const string NamedPeople = "N";
        }

        public static class ElectionMode
        {
            public const string Normal = "N";
            public const string TieBreak = "T";
            public const string ByElection = "B";
        }
    }
}