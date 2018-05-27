namespace BugHub.Data.Context
{
  public class DbContextFactory : IDbContextFactory
  {
    private readonly SqlConnectionOptions _sqlConnectionOptions;

    public DbContextFactory(SqlConnectionOptions sqlConnectionOptions)
    {
      _sqlConnectionOptions = sqlConnectionOptions;
    }

    public BugDbContext CreateBugDbContext()
    {
      return new BugDbContext(_sqlConnectionOptions);
    }
  }
}