using System;
using BugHub.Data.Context;
using BugHub.Data.Entities;

namespace BugHub.Data.Repositories
{
  public class BugRepository : IBugRepository
  {
    private readonly IDbContextFactory dbContextFactory;

    public BugRepository(IDbContextFactory dbContextFactory)
    {
      this.dbContextFactory = dbContextFactory;
    }

    public BugEntity Create(BugEntity bugEntity)
    {
      using (var ctx = dbContextFactory.CreateBugDbContext())
      {
        bugEntity.CreationDate = DateTime.UtcNow;
        bugEntity.LastModificationDate = DateTime.UtcNow;

        var updatedButEntity = ctx.Bugs.Add(bugEntity);
        ctx.SaveChanges();

        return updatedButEntity;
      }
    }
  }
}