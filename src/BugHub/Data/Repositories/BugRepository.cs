using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using BugHub.Data.Context;
using BugHub.Data.Entities;

namespace BugHub.Data.Repositories
{
  public class BugRepository : IBugRepository
  {
    private readonly IDbContextFactory _dbContextFactory;

    public BugRepository(IDbContextFactory dbContextFactory)
    {
      _dbContextFactory = dbContextFactory;
    }

    public async Task<BugEntity> Create(BugEntity bugEntity)
    {
      using (var ctx = _dbContextFactory.CreateBugDbContext())
      {
        bugEntity.CreationDate = DateTime.UtcNow;
        bugEntity.LastModificationDate = DateTime.UtcNow;

        var updatedButEntity = ctx.Bugs.Add(bugEntity);
        await ctx.SaveChangesAsync();

        return updatedButEntity;
      }
    }

    public async Task<IList<BugEntity>> Get()
    {
      using (var ctx = _dbContextFactory.CreateBugDbContext())
      {
        return await ctx.Bugs.ToListAsync();
      }
    }

    public async Task<BugEntity> Update(BugEntity bugEntity)
    {
      using (var ctx = _dbContextFactory.CreateBugDbContext())
      {
        var existingBug = await ctx.Bugs.SingleOrDefaultAsync(x => x.Id == bugEntity.Id);
        if (existingBug == null)
        {
          return null;
        }
        
        existingBug.LastModificationDate = DateTime.UtcNow;
        existingBug.Title = bugEntity.Title;
        existingBug.Description = bugEntity.Description;
        
        await ctx.SaveChangesAsync();

        return existingBug;
      }
    }

    public async Task<bool> Delete(long id)
    {
      using (var ctx = _dbContextFactory.CreateBugDbContext())
      {
        var existingBug = await ctx.Bugs.SingleOrDefaultAsync(x => x.Id == id);
        if (existingBug == null)
        {
          return false;
        }

        ctx.Bugs.Remove(existingBug);
        
        await ctx.SaveChangesAsync();

        return true;
      }
    }
  }
}