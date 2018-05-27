using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using BugHub.Data.Context;
using BugHub.Data.Repositories;
using BugHub.WebApi;
using BugHub.WebApi.Controllers;
using FluentAssertions;
using Ninject;
using Ninject.Activation;
using Xunit;

namespace BugHub.UnitTests
{
  public class NinjectKernelTests
  {
    private readonly IKernel _kernel = new StandardKernel();
    private readonly Func<IContext, object> _singletonScope;

    public NinjectKernelTests()
    {
      NinjectConfig.RegisterServices(_kernel);

      using (var kernel = new StandardKernel())
      {
        kernel.Bind<object>().ToSelf().InSingletonScope();
        _singletonScope = kernel.GetBindings(typeof(object)).Single().ScopeCallback;
      }
    }

    [Theory]
    [InlineData(typeof(IMapper))]
    [InlineData(typeof(IBugRepository))]
    [InlineData(typeof(IDbContextFactory))]
    public void RegistersExpectedSingletons(Type type)
    {
      _kernel.GetBindings(type).Single().ScopeCallback.Should().Be(_singletonScope);
    }

    [Theory]
    [InlineData(typeof(BugsController))]
    public void ResolvesEntryPoints(Type type)
    {
      ConstructorInfo constructor = type.GetConstructorWithLongestParamList();
      var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType).ToList();
      var parameters = new List<object>();
      foreach (var parameterType in parameterTypes)
      {
        var resolvedType = _kernel.Get(parameterType);
        parameters.Add(resolvedType);
      }

      var entyPoint = constructor.Invoke(parameters.ToArray());
      entyPoint.Should().NotBeNull();
      entyPoint.GetType().Should().BeSameAs(type);
    }
  }
}
