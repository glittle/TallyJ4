using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TallyJ3.Data.DbModel;

namespace TallyJ3.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Ballot> Ballot { get; set; }

        // Computer is not saved
        public DbSet<Election> Election { get; set; }

        public DbSet<Location> Location { get; set; }

        public DbSet<JoinElectionUser> JoinElectionUser { get; set; }


        public DbSet<OnlineTempBallot> OnlineTempBallots { get; set; }

        public DbSet<Person> Person { get; set; }

        public DbSet<Result> Result { get; set; }

        public DbSet<ResultSummary> ResultSummary { get; set; }

        public DbSet<ResultTie> ResultTie { get; set; }

        public DbSet<Teller> Teller { get; set; }

        public DbSet<Vote> Vote { get; set; }

        public DbSet<Log> Log { get; set; }

    }
}
