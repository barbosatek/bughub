using System;
using AutoFixture;
using BugHub.Data.Entities;
using BugHub.WebApi.Models.V1;

namespace BugHub.UnitTests
{
  public static class TestUtility
  {
    public static readonly Fixture FixtureInstance = new Fixture();

    public static Uri CreateUri()
    {
      return new Uri("http://" + FixtureInstance.Create<string>() + ".com");
    }

    public static string RandomString => FixtureInstance.Create<string>();

    public static BugEntity GenerateEntityBugLite()
    {
      return new BugEntity
      {
        Title = FixtureInstance.Create<string>(),
        Description = FixtureInstance.Create<string>()
      };
    }

    public static BugEntity GenerateEntityBug()
    {
      return new BugEntity
      {
        Title = FixtureInstance.Create<string>(),
        Description = FixtureInstance.Create<string>(),
        Id = FixtureInstance.Create<long>(),
        CreationDate = FixtureInstance.Create<DateTime>(),
        LastModificationDate = FixtureInstance.Create<DateTime>()
      };
    }

    public static Bug GenerateBug()
    {
      return new Bug
      {
        Title = FixtureInstance.Create<string>(),
        Description = FixtureInstance.Create<string>(),
        Id = FixtureInstance.Create<long>(),
        CreationDate = FixtureInstance.Create<DateTime>()
      };
    }
  }
}
