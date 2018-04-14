using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using TallyJ4.Code.Session;
using TallyJ4.Data.DbModel;
using TallyJ4.Code.Misc;

namespace TallyJ4.Data.Caching
{
    public abstract class CacherBase
    {
        private Guid _currentElectionGuid;
        protected abstract object LockCacheBaseObject { get; }

        public Guid CurrentElectionGuid
        {
            get
            {
                return _currentElectionGuid != Guid.Empty
                  ? _currentElectionGuid
                  : (_currentElectionGuid = UserSession.CurrentElectionGuid);
            }
        }

    }

    public abstract class CacherBase<T> : CacherBase, ICacherBase<T> where T : class, IIndexedForCaching
    {
        private const int CacheMinutes = 30; // long enough for a reasonable gap in usage
        private IMemoryCache Cache;

        public CacherBase(ApplicationDbContext dbContext)
        {
            CurrentDb = dbContext;
            Cache = Startup.ServiceProvider.GetService<IMemoryCache>();
        }

        /// <summary>
        ///   The key for the current election's data
        /// </summary>
        protected virtual string CacheKeyRaw
        {
            get { return typeof(T).Name + CurrentElectionGuid; }
        }

        protected ApplicationDbContext CurrentDb { get; set; }

        /// <summary>
        ///   Get a single <typeparamref name="T" /> by Id. If not found, returns null
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public T GetById(int rowId)
        {
            return AllForThisElection.FirstOrDefault(t => t.Id == rowId);
        }

        public List<T> AllForThisElection
        {
            get
            {
                var db = CurrentDb;

                List<T> allForThisElection;

                lock (LockCacheBaseObject)
                {
                    if (!Cache.TryGetValue<List<T>>(CacheKeyRaw, out allForThisElection))
                    {
                        allForThisElection = MainQuery().ToList();

                        // Set cache options.
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            // Keep in cache for this time, reset time if accessed.
                            .SetSlidingExpiration(TimeSpan.FromMinutes(CacheMinutes));

                        // Save data in cache.
                        Cache.Set(CacheKeyRaw, allForThisElection, cacheEntryOptions);

                        // in a Lock, so can update the list
                        var dict = Cache.GetOrCreate(CurrentElectionGuid, e => new Dictionary<string, bool>());
                        dict[CacheKeyRaw] = true;
                        Cache.Set(CurrentElectionGuid, dict);
                    }
                    //allForThisElection = MainQuery()
                    //  .FromCache(CachePolicy.WithSlidingExpiration(TimeSpan.FromMinutes(CacheMinutes)),
                    //    new[] { CacheKeyRaw, CurrentElectionGuid.ToString() }).ToList();
                }
                return allForThisElection;
            }
        }

        /// <summary>
        ///   Find the item by matching the _RowId (if found), remove it, then replace it with this one.
        ///   Can be used to Add or Update
        /// </summary>
        /// <param name="replacementItem"></param>
        public ICacherBase<T> UpdateItemAndSaveCache(T replacementItem)
        {
            AssertAtRuntime.That(replacementItem.Id != 0, "Can't add if id is 0");

            lock (LockCacheBaseObject)
            {
                var list = AllForThisElection;

                var oldItem = list.FirstOrDefault(i => i.Id == replacementItem.Id);
                if (oldItem != null)
                {
                    list.Remove(oldItem);
                }

                list.Add(replacementItem);

                ReplaceEntireCache(list);
            }

            return this;
        }


        public void RemoveItemsAndSaveCache(IEnumerable<T> itemsToRemove)
        {
            var removed = false;

            var ids = itemsToRemove.Select(i => i.Id).ToList();

            lock (LockCacheBaseObject)
            {
                var list = AllForThisElection;
                var oldItems = list.Where(i => ids.Contains(i.Id)).ToList();
                foreach (var item in oldItems)
                {
                    list.Remove(item);
                    removed = true;
                }

                if (removed)
                {
                    ReplaceEntireCache(list);
                }
            }
        }

        /// <summary>
        ///   Find the item by matching the _RowId, remove if found
        /// </summary>
        /// <param name="itemToRemove"></param>
        public ICacherBase<T> RemoveItemAndSaveCache(T itemToRemove)
        {
            lock (LockCacheBaseObject)
            {
                var list = AllForThisElection;
                var oldItem = list.FirstOrDefault(i => i.Id == itemToRemove.Id);
                if (oldItem != null)
                {
                    list.Remove(oldItem);
                    ReplaceEntireCache(list);
                }
            }
            return this;
        }

        /// <summary>
        ///   Put the (modified) List back into the cache
        /// </summary>
        /// <param name="listFromCache"></param>
        public void ReplaceEntireCache(List<T> listFromCache)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromMinutes(CacheMinutes));

            // Save data in cache.
            Cache.Set(CacheKeyRaw, listFromCache, cacheEntryOptions);

            //var key = new CacheKey(MainQuery().GetCacheKey(),
            //  new[] { CacheKeyRaw, CurrentElectionGuid.ToString() });

            //CacheManager.Current.Set(key, listFromCache, CachePolicy.WithSlidingExpiration(TimeSpan.FromMinutes(CacheMinutes)));

            ItemChanged();
        }

        /// <summary>
        ///   Add this item to the cached list. The cache is updated with the current version of the data.
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns></returns>
        /// <summary>
        ///   Drop the cache of
        ///   <typeparam name="T"></typeparam>
        ///   for this election
        /// </summary>
        public ICacherBase<T> DropThisCache()
        {
            Cache.Remove(CacheKeyRaw);

            //CacheManager.Current.Expire(CacheKeyRaw);
            return this;
        }

        /// <summary>
        ///   When an item has been added or changed
        /// </summary>
        protected virtual void ItemChanged()
        {
        }

        public abstract IQueryable<T> MainQuery();
    }
}