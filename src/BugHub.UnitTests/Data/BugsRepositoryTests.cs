﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BugHub.Data.Context;
using BugHub.Data.Entities;
using BugHub.Data.Repositories;
using Effort;
using FluentAssertions;
using Xunit;

namespace BugHub.UnitTests.Data
{
  public class BugsRepositoryTests : MoqTestFor<BugRepository>
  {
    [Fact]
    public async Task GivenABugRepository_WhenBugIsCreated_ThenReturnsCreatedBug()
    {
      // Arrange
      var connection = DbConnectionFactory.CreateTransient();
      The<IDbContextFactory>().Setup(x => x.CreateBugDbContext()).Returns(new BugDbContext(connection));
      var expectedBug = TestUtility.GenerateBug();

      // Act
      var actualBug = await Target.Create(expectedBug);

      // Assert
      actualBug.Should().NotBeNull();
      actualBug.Title.Should().Be(expectedBug.Title);
      actualBug.Description.Should().Be(expectedBug.Description);
      actualBug.CreationDate.Should().BeAfter(DateTime.MinValue);
      actualBug.LastModificationDate.Should().BeAfter(DateTime.MinValue);
      actualBug.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GivenABugRepository_WhenBugsAreRetrieved_ThenReturnsBugs()
    {
      // Arrange
      var connection = DbConnectionFactory.CreateTransient();
      var dbContext = new BugDbContext(connection);
      The<IDbContextFactory>().Setup(x => x.CreateBugDbContext()).Returns(dbContext);
      var expectedBugs = Enumerable.Range(0, 10).Select(x => TestUtility.GenerateBug()).ToList();
      foreach (var bugEntity in expectedBugs)
      {
        dbContext.Bugs.Add(bugEntity);
      }

      await dbContext.SaveChangesAsync();

      // Act
      var actualBugs = await Target.Get();

      // Assert
      actualBugs.Count.Should().Be(expectedBugs.Count);

      foreach (var actualBug in actualBugs)
      {
        var expectedBug = expectedBugs.SingleOrDefault(x => x.Id == actualBug.Id);
        expectedBug.Should().NotBeNull();

        actualBug.CreationDate.Should().Be(expectedBug.CreationDate);
        actualBug.LastModificationDate.Should().Be(expectedBug.LastModificationDate);
        actualBug.Title.Should().Be(expectedBug.Title);
        actualBug.Description.Should().Be(expectedBug.Description);
      }
    }
  }
}