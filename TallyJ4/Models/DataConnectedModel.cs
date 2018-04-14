
using Microsoft.EntityFrameworkCore;
using TallyJ4.Code.Session;
using TallyJ4.Data;

namespace TallyJ4.Models
{
    public abstract class DataConnectedModel
    {
        private ApplicationDbContext _db;

        /// <summary>
        ///     Access to the database
        /// </summary>
        protected ApplicationDbContext GetNewDbContext()
        {
            return UserSession.GetNewDbContext();
        }

        protected ApplicationDbContext SharedDbContext
        {
            get
            {
                return _db ?? (_db = GetNewDbContext());
            }
            set {
                _db = value;
            }
        }
    }
}