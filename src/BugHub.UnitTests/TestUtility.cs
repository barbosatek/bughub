using AutoFixture;
using BugHub.Data.Entities;

namespace BugHub.UnitTests
{
  public static class TestUtility
  {
    public static readonly Fixture FixtureInstance = new Fixture();

    public static BugEntity GenerateBug()
    {
      return new BugEntity
      {
        Title = FixtureInstance.Create<string>(),
        Description = FixtureInstance.Create<string>()
      };
    }
  }
}
