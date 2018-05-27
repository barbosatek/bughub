using System.Collections.Generic;
using System.Threading.Tasks;
using BugHub.Data.Entities;

namespace BugHub.Data.Repositories
{
  public interface IBugRepository
  {
    Task<BugEntity> Create(BugEntity bugEntity);
    Task<IList<BugEntity>> Get();
    Task<BugEntity> Update(BugEntity bugEntity);
    Task<bool> Delete(long id);
  }
}