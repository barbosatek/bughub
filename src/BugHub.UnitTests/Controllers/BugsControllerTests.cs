using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoMapper;
using BugHub.Data.Entities;
using BugHub.Data.Repositories;
using BugHub.WebApi.Controllers;
using BugHub.WebApi.Models.V1;
using FluentAssertions;
using Moq;
using Xunit;
using AutoFixture;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace BugHub.UnitTests.Controllers
{
  public class BugsControllerTests : MoqTestFor<BugsController>
  {
    [Fact]
    public async Task GivenTheServer_WhenGetRequestIsProcessed_ReturnsBugs()
    {
      // Arrange
      var bugEntities = Enumerable.Range(0, 5).Select(x => TestUtility.GenerateEntityBug()).ToList();
      The<IBugRepository>().Setup(x => x.Get()).Returns(Task.FromResult(bugEntities));
      The<IMapper>().Setup(x => x.Map<Bug>(It.Is<BugEntity>(p => bugEntities.Any(bugEntity => bugEntity.Id == p.Id))))
        .Returns<BugEntity>(b =>
        {
          var generateBug = TestUtility.GenerateBug();
          generateBug.Id = b.Id;
          return generateBug;
        });
      
      Target.SetupRequest(HttpMethod.Get, TestUtility.CreateUri());

      // Act
      var result = await Target.Get() as OkNegotiatedContentResult<List<Bug>>;
      
      // Assert
      result.Should().NotBeNull();
      result.Content.Should().NotBeNull();
      foreach (var bug in result.Content)
      {
        bugEntities.Any(x => x.Id == bug.Id).Should().BeTrue();
      }
    }

    [Fact]
    public async Task GivenTheServer_WhenPutRequestIsProcessed_ReturnsBug()
    {
      // Arrange
      var bug = TestUtility.GenerateBug();
      var bugEntity = TestUtility.GenerateEntityBug();
      The<IBugRepository>().Setup(x => x.Create(bugEntity)).Returns(Task.FromResult(bugEntity));
      The<IMapper>().SetupMap(bug, bugEntity);
      
      Target.SetupRequest(HttpMethod.Put, TestUtility.CreateUri());

      // Act
      var result = await Target.Put(bug) as CreatedNegotiatedContentResult<Bug>;

      // Assert
      result.Should().NotBeNull();
      result.Content.Should().NotBeNull();
      result.Content.Should().Be(bug);
    }

    [Fact]
    public async Task GivenTheServer_WhenGetBugRequestIsProcessed_ReturnsBug()
    {
      // Arrange
      var bug = TestUtility.GenerateBug();
      var bugEntity = TestUtility.GenerateEntityBug();
      bug.Id = bugEntity.Id;
      The<IBugRepository>().Setup(x => x.Get(bugEntity.Id)).Returns(Task.FromResult(bugEntity));
      The<IMapper>().SetupMap(bug, bugEntity);
      
      Target.SetupRequest(HttpMethod.Get, TestUtility.CreateUri());

      // Act
      var result = await Target.Get(bug.Id) as OkNegotiatedContentResult<Bug>;

      // Assert
      result.Should().NotBeNull();
      result.Content.Should().NotBeNull();
      result.Content.Should().Be(bug);
    }

    [Fact]
    public async Task GivenTheServer_WhenGetBugRequestIsProcessedAndBugDoesNotExist_ReturnsNotFound()
    {
      // Arrange
      var id = TestUtility.FixtureInstance.Create<long>();
      BugEntity bugEntity = null;
      The<IBugRepository>().Setup(x => x.Get(id)).Returns(Task.FromResult(bugEntity));
      
      Target.SetupRequest(HttpMethod.Get, TestUtility.CreateUri());

      // Act
      var result = await Target.Get(id) as NotFoundResult;

      // Assert
      result.Should().NotBeNull();
    }

    [Fact]
    public async Task GivenTheServer_WhenDeleteBugRequestIsProcessed_ReturnsNoContent()
    {
      // Arrange
      var id = TestUtility.FixtureInstance.Create<long>();
      The<IBugRepository>().Setup(x => x.Delete(id)).Returns(Task.FromResult(true));
      
      Target.SetupRequest(HttpMethod.Delete, TestUtility.CreateUri());

      // Act
      var result = await Target.Delete(id) as ResponseMessageResult;

      // Assert
      result.Should().NotBeNull();
      result.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenTheServer_WhenPatchBugRequestIsProcess_ReturnsNoContent()
    {
      // Arrange
      var bug = TestUtility.GenerateBug();
      var bugEntity = TestUtility.GenerateEntityBug();
      bug.Id = bugEntity.Id;

      The<IBugRepository>().Setup(x => x.Get(bug.Id)).Returns(Task.FromResult(bugEntity));
      The<IBugRepository>().Setup(x => x.Update(bugEntity)).Returns(Task.FromResult(bugEntity));
      The<IMapper>().SetupMap(bug, bugEntity);
      
      Target.SetupRequest(HttpMethod.Put, TestUtility.CreateUri());
      var patchDocument = new JsonPatchDocument<Bug>();

      // Act
      var result = await Target.Patch(patchDocument, bug.Id) as ResponseMessageResult;

      // Assert
      result.Should().NotBeNull();
      result.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenTheServer_WhenPatchBugRequestIsProcessAndBugDoesNotExist_ReturnsNoFound()
    {
      // Arrange
      BugEntity bugEntity = null;
      var id = TestUtility.FixtureInstance.Create<long>();
      The<IBugRepository>().Setup(x => x.Get(id)).Returns(Task.FromResult(bugEntity));
      
      Target.SetupRequest(HttpMethod.Put, TestUtility.CreateUri());
      
      // Act
      var result = await Target.Patch(new JsonPatchDocument<Bug>(), id) as NotFoundResult;

      // Assert
      result.Should().NotBeNull();
    }

    [Fact]
    public async Task GivenTheServer_WhenPatchBugRequestWithErrorsIsProcess_ReturnsBadRequest()
    {
      // Arrange
      var bug = TestUtility.GenerateBug();
      var bugEntity = TestUtility.GenerateEntityBug();
      bug.Id = bugEntity.Id;

      The<IBugRepository>().Setup(x => x.Get(bug.Id)).Returns(Task.FromResult(bugEntity));
      The<IMapper>().SetupMap(bug, bugEntity);
      
      Target.SetupRequest(HttpMethod.Put, TestUtility.CreateUri());
      var patchDocument = new JsonPatchDocument<Bug>();
      patchDocument.Operations.Add(new Operation<Bug>(TestUtility.RandomString, TestUtility.RandomString, TestUtility.RandomString));

      // Act
      var result = await Target.Patch(patchDocument, bug.Id) as BadRequestErrorMessageResult;

      // Assert
      result.Should().NotBeNull();
      result.Message.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("/id")]
    [InlineData("/Id")]
    public async Task GivenTheServer_WhenPatchBugRequestForForbiddenPathIsProcess_ReturnsForbidden(string path)
    {
      // Arrange
      var id = TestUtility.FixtureInstance.Create<long>();
      Target.SetupRequest(HttpMethod.Put, TestUtility.CreateUri());
      var patchDocument = new JsonPatchDocument<Bug>();
      patchDocument.Operations.Add(new Operation<Bug>("replace", path, TestUtility.FixtureInstance.Create<long>().ToString()));

      // Act
      var result = await Target.Patch(patchDocument, id) as ResponseMessageResult;

      // Assert
      result.Should().NotBeNull();
      result.Response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
  }
}