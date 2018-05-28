using AutoMapper;
using BugHub.Data.Entities;
using BugHub.WebApi.Models.V1;
using Moq;

namespace BugHub.UnitTests.Controllers
{
  public static class MapperTestExtensions
  {
    public static void SetupMap(this Mock<IMapper> mock, Bug bug, BugEntity bugEntity)
    {
      mock.Setup(x => x.Map<Bug>(It.Is<BugEntity>(p => p.Id == bugEntity.Id))).Returns(bug);
      mock.Setup(x => x.Map<BugEntity>(It.Is<Bug>(p => p.Id == bug.Id))).Returns(bugEntity);
    }
  }
}