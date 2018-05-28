using System.Data.Common;
using System.Data.Entity;
using BugHub.Data.Entities;

namespace BugHub.Data.Context
{
  public partial class BugDbContext : DbContext
  {
    public BugDbContext(SqlConnectionOptions sqlConnectionOptions) : base(StringConectionFactory.Create(sqlConnectionOptions))
    {
    }

    public BugDbContext(DbConnection dbConnection) : base(dbConnection, true)
    {
      // For testing purposes.
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      Database.SetInitializer<BugDbContext>(null);
      base.OnModelCreating(modelBuilder);
    }

    public IDbSet<BugEntity> Bugs { get; set; }
  }
}
