using BugHub.Data.Entities;

namespace BugHub.Data.Repositories
{
  public interface IBugRepository
  {
    BugEntity Create(BugEntity bugEntity);
  }
}