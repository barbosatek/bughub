using System.Data.Common;
using System.Data.Entity;
using BugHub.Data.Entities;

namespace BugHub.Data.Context
{
  public partial class BugDbContext : DbContext
  {
    public BugDbContext() : base()
    {
    }

    public BugDbContext(DbConnection dbConnection) : base(dbConnection, true)
    {
      // For testing purposes.
    }

    public IDbSet<BugEntity> Bugs { get; set; }
  }
}
