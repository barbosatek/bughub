namespace BugHub.Data.Context
{
  public interface IDbContextFactory
  {
    BugDbContext CreateBugDbContext();
  }
}