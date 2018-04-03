using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TallyJ3.Code.Enumerations;
using TallyJ3.Code.Helper;
using TallyJ3.Code.Session;
using TallyJ3.Data.Caching;
using TallyJ3.Data.DbModel;
using TallyJ3.Extensions;

namespace TallyJ3.Models
{
    public class LocationModel : DataConnectedModel
    {
        private List<Location> _locations;
        private Dictionary<Guid, int> _idMap;

        /// <Summary>List of Locations</Summary>
        public List<Location> AllLocations
        {
            get { return _locations ?? (_locations = new LocationCacher(SharedDbContext).AllForThisElection); }
        }

        public Dictionary<Guid, int> LocationIdMap
        {
            get
            {
                return _idMap ?? (_idMap = AllLocations.ToDictionary(l => l.LocationGuid, l => l.Id));
            }
        }
        public string LocationRowIdMap
        {
            get
            {
                return AllLocations
                  .Select(l => "{0}:{1}".FilledWith(l.Id, l.Name.SerializedAsJsonString()))
                  .JoinedAsString(", ")
                  .SurroundContentWith("{", "}");
            }
        }


        /// <summary>
        /// Get the RowId from a LocationGuid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int IdFor(Guid? guid, int defaultValue = 0)
        {
            if (!guid.HasValue)
            {
                return defaultValue;
            }
            if (LocationIdMap.ContainsKey(guid.Value))
            {
                return LocationIdMap[guid.Value];
            }
            return defaultValue;
        }

        public string ShowDisabled
        {
            get { return AllLocations.Count == 1 ? " disabled" : ""; }
        }

        /// <Summary>Does this election have more than one location?</Summary>
        public bool HasLocations
        {
            get { return AllLocations.Count > 1; }
        }

        public HtmlString GetLocationOptions(bool includeWhichIfNeeded = true)
        {
            var currentLocation = UserSession.CurrentLocation;
            var selected = 0;
            if (currentLocation != null)
            {
                selected = currentLocation.Id;
            }

            return
              (
              (selected == 0 && includeWhichIfNeeded ? "<option value='-1'>Which Location?</option>" : "") +
              AllLocations
              .OrderBy(l => l.SortOrder)
              .Select(l => new { l.Id, l.Name, Selected = l.Id == selected ? " selected" : "" })
              .Select(l => "<option value={Id}{Selected}>{Name}</option>".FilledWith(l))
              .JoinedAsString())
              .AsRawHtml();
        }

        //TODO
        /// <Summary>Does this page need to show the location selector?</Summary>
        //public bool ShowLocationSelector(MenuHelper currentMenu)
        //{
        //    return currentMenu.ShowLocationSelection && HasLocations;
        //}

        public JsonResult UpdateStatus(int locationId, string status)
        {
            var locationCacher = new LocationCacher(SharedDbContext);

            var location = AllLocations.SingleOrDefault(l => l.Id == locationId);

            if (location == null)
            {
                return new
                {
                    Saved = false
                }.AsJsonResult();
            }

            if (location.TallyStatus != status)
            {
                SharedDbContext.Location.Attach(location);
                location.TallyStatus = status;
                SharedDbContext.SaveChanges();

                locationCacher.UpdateItemAndSaveCache(location);
            }

            return new
            {
                Saved = true,
                Location = LocationInfoForJson(location)
            }.AsJsonResult();
        }

        public string CurrentBallotLocationJsonString()
        {
            return CurrentBallotLocationInfo().SerializedAsJsonString();
        }

        public object CurrentBallotLocationInfo()
        {
            return LocationInfoForJson(UserSession.CurrentLocation);
        }

        public object LocationInfoForJson(Location location)
        {
            if (location == null)
            {
                return null;
            }

            var isSingleName = UserSession.CurrentElection.IsSingleNameElection;
            var sum = new BallotHelper().BallotCount(location.LocationGuid, isSingleName);

            return new
            {
                Id = location.Id,
                TallyStatus = location == null ? "" : LocationStatusEnum.TextFor(location.TallyStatus),
                TallyStatusCode = location.TallyStatus,
                location.ContactInfo,
                location.BallotsCollected,
                location.Name,
                BallotsEntered = sum
            };
        }

        public JsonResult UpdateNumCollected(int numCollected)
        {
            var location = UserSession.CurrentLocation;

            if (location == null)
            {
                return new { Message = "Must select your location first!" }.AsJsonResult();
            }

            SharedDbContext.Location.Attach(location);

            location.BallotsCollected = numCollected;

            SharedDbContext.SaveChanges();

            new LocationCacher(SharedDbContext).UpdateItemAndSaveCache(location);

            return new
            {
                Saved = true,
                Location = LocationInfoForJson(location)
            }.AsJsonResult();
        }


        public JsonResult UpdateLocationInfo(string info)
        {
            var location = UserSession.CurrentLocation;
            SharedDbContext.Location.Attach(location);

            location.ContactInfo = info;

            SharedDbContext.SaveChanges();

            new LocationCacher(SharedDbContext).UpdateItemAndSaveCache(location);

            return new { Saved = true }.AsJsonResult();
        }

        public JsonResult EditLocation(int id, string text)
        {
            var locationCacher = new LocationCacher(SharedDbContext);

            var location = locationCacher.AllForThisElection.SingleOrDefault(l => l.Id == id);
            var changed = false;

            if (location == null)
            {
                location = new Location
                {
                    ElectionGuid = UserSession.CurrentElectionGuid,
                    LocationGuid = Guid.NewGuid()
                };
                SharedDbContext.Location.Add(location);
                changed = true;
            }
            else
            {
                SharedDbContext.Location.Attach(location);
            }

            int locationId;
            string locationText;
            string status;
            var success = false;

            if (text.HasNoContent() && location.Id > 0)
            {
                // don't delete last location
                if (AllLocations.Count() > 1)
                {
                    // delete existing if we can
                    var used = new BallotCacher(SharedDbContext).AllForThisElection.Any(b => b.LocationGuid == location.LocationGuid);
                    if (!used)
                    {
                        SharedDbContext.Location.Remove(location);
                        SharedDbContext.SaveChanges();
                        locationCacher.RemoveItemAndSaveCache(location);

                        status = "Deleted";
                        success = true;
                        locationId = 0;
                        locationText = "";
                    }
                    else
                    {
                        status = "Cannot deleted this location because it has Ballots recorded in it";
                        locationId = location.Id;
                        locationText = location.Name;
                    }
                }
                else
                {
                    // only one
                    status = "At least one location is required";
                    locationId = location.Id;
                    locationText = location.Name;
                }
            }
            else if (text.HasContent())
            {
                locationText = location.Name = text;
                locationId = location.Id; // may be 0 if new

                changed = true;
                status = "Saved";
            }
            else
            {
                status = "Nothing to save";
                locationId = 0;
                locationText = "";
                success = true;
                changed = false;
            }

            if (changed)
            {
                SharedDbContext.SaveChanges();

                locationId = location.Id;
                locationCacher.UpdateItemAndSaveCache(location);
                success = true;
            }

            return new
            {
                // returns 0 if deleted or not created
                Id = locationId,
                Text = locationText,
                Success = success,
                Status = status
            }.AsJsonResult();
        }

        public JsonResult SortLocations(List<int> idList)
        {
            //var ids = idList.Split(new[] { ',' }).AsInts().ToList();

            var locationCacher = new LocationCacher(SharedDbContext);

            var locations = locationCacher.AllForThisElection.Where(l => idList.Contains(l.Id)).ToList();

            var sortOrder = 1;
            foreach (var id in idList)
            {
                var newOrder = sortOrder++;

                var location = locations.SingleOrDefault(l => l.Id == id);

                if (location != null && location.SortOrder != newOrder)
                {
                    SharedDbContext.Location.Attach(location);
                    location.SortOrder = newOrder;

                    locationCacher.UpdateItemAndSaveCache(location);
                }
            }

            SharedDbContext.SaveChanges();

            return new
            {
                Saved = true
            }.AsJsonResult();
        }
    }
}