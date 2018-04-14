using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TallyJ4.Data.DbModel;

namespace TallyJ4.Data
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

      builder.ApplyConfiguration(new ElectionConfiguration());
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

  public class MigrationsContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
  {
    public ApplicationDbContext CreateDbContext(string[] args)
    {
      var cnString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-TallyJ4-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true";

      var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

      optionsBuilder.UseSqlServer(cnString, 
        opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));

      return new ApplicationDbContext(optionsBuilder.Options);
    }
  }
}
