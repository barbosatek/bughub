using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using BugHub.Data.Entities;
using BugHub.Data.Repositories;
using BugHub.WebApi.Controllers;
using BugHub.WebApi.Models.V1;
using FluentAssertions;
using Moq;
using Xunit;

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
      The<IMapper>().Setup(x => x.Map<Bug>(It.Is<BugEntity>(p => p.Id == bugEntity.Id))).Returns(bug);
      The<IMapper>().Setup(x => x.Map<BugEntity>(It.Is<Bug>(p => p.Id == bug.Id))).Returns(bugEntity);
      
      Target.SetupRequest(HttpMethod.Get, TestUtility.CreateUri());

      // Act
      var result = await Target.Put(bug) as CreatedNegotiatedContentResult<Bug>;

      // Assert
      result.Should().NotBeNull();
      result.Content.Should().NotBeNull();
      result.Content.Should().Be(bug);
    }
  }

  public static class ApiControllerTestExtensions
  {
    public static void SetupRequest<T>(this T controller, HttpMethod httpMethod, Uri uri) where T : ApiController
    {
      controller.Request = new HttpRequestMessage(httpMethod, uri);
    }
  }
}