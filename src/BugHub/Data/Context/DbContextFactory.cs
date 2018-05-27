namespace BugHub.Data.Context
{
  public class DbContextFactory : IDbContextFactory
  {
    public BugDbContext CreateBugDbContext()
    {
      return new BugDbContext();
    }
  }
}