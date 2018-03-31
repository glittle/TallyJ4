
using Microsoft.EntityFrameworkCore;
using TallyJ3.Data;

namespace TallyJ3.Code.Models
{
    public abstract class DataConnectedModel
    {
        /// <summary>
        ///     Access to the database
        /// </summary>
        protected ApplicationDbContext Db()
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        }
    }
}