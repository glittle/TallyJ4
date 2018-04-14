
using Microsoft.EntityFrameworkCore;
using TallyJ3.Code.Session;
using TallyJ3.Data;

namespace TallyJ3.Models
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