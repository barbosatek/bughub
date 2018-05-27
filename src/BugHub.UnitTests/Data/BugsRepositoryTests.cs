using System;
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
    public void GivenABugRepository_WhenBugIsCreated_ReturnsCreatedBug()
    {
      // Arrange
      var connection = DbConnectionFactory.CreateTransient();
      The<IDbContextFactory>().Setup(x => x.CreateBugDbContext()).Returns(new BugDbContext(connection));
      var expectedBug = new BugEntity
      {
        Title = TestUtility.FixtureInstance.Create<string>(),
        Description = TestUtility.FixtureInstance.Create<string>()
      };

      // Act
      var actualBug = Target.Create(expectedBug);

      // Assert
      actualBug.Should().NotBeNull();
      actualBug.Title.Should().Be(expectedBug.Title);
      actualBug.Description.Should().Be(expectedBug.Description);
      actualBug.CreationDate.Should().BeAfter(DateTime.MinValue);
      actualBug.LastModificationDate.Should().BeAfter(DateTime.MinValue);
      actualBug.Id.Should().BeGreaterThan(0);
    }
  }
}