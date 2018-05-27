using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Moq;

namespace BugHub.UnitTests
{
  // Code from https://github.com/barbosatek/Softmex.Test
  // Currently, it doesn't create a nuget package.
  // Potentially, it could use MyGet to create its package off github
  // See https://weblogs.asp.net/bsimser/automatically-publishing-nuget-packages-from-github
  public class MoqTestFor<T> : TestFor<T>, IDisposable where T : class
  {
    private readonly IDictionary<Type, Mock> _mocks;

    public MoqTestFor()
    {
      _mocks = new Dictionary<Type, Mock>();
    }

    public new void Dispose()
    {
      _mocks.Clear();
      base.Dispose();
    }

    public Mock<T2> The<T2>() where T2 : class
    {
      return (Mock<T2>)The(typeof(T2));
    }

    public Mock The(Type type)
    {
      if (!_mocks.ContainsKey(type))
      {
        _mocks.Add(type, BuildMock(type));
      }

      return _mocks[type];
    }

    protected override IDictionary<Type, object> GetMockedDependencies()
    {
      return _mocks.ToDictionary(x => x.Key, x => x.Value.Object);
    }

    protected override object MockDependency(Type type)
    {
      var mock = BuildMock(type);

      if (!_mocks.ContainsKey(type))
      {
        _mocks.Add(type, mock);
      }

      return mock.Object;
    }

    private Mock BuildMock(Type objectType)
    {
      var mockType = typeof(Mock<>).MakeGenericType(objectType);
      return (Mock)Activator.CreateInstance(mockType);
    }
  }

  public abstract class TestFor<T> : IDisposable where T : class
  {
    public T Target => _target ?? (_target = BuildTarget());
    
    private T _target;
    protected abstract IDictionary<Type, object> GetMockedDependencies();
    protected abstract object MockDependency(Type type);

    private readonly Dictionary<Type, object> _dependencies;

    protected TestFor()
    {
      _dependencies = new Dictionary<Type, object>();
    }

    public void SetDependency<T2>(T2 dependency) where T2 : class
    {
      var type = typeof(T2);
      SetDependency(type, dependency);
    }

    public void SetDependency(Type type, object dependency)
    {
      if (!_dependencies.ContainsKey(type))
      {
        _dependencies.Add(type, dependency);
      }
      else
      {
        _dependencies[type] = dependency;
      }
    }

    public T2 GetDependency<T2>() where T2 : class
    {
      var type = typeof(T2);
      if (!_dependencies.ContainsKey(type))
      {
        return null;
      }
      else
      {
        return (T2)_dependencies[type];
      }
    }

    private T BuildTarget()
    {
      ConstructorInfo constructor = TypeUtility.GetConstructorWithLongestParamList(typeof(T));
      List<Type> parameterTypes = constructor.GetParameters().Select(x => x.ParameterType).ToList();
      var mocks = GetMockedDependencies();

      if (parameterTypes.Count > 0)
      {
        var parameters = new List<object>();
        foreach (Type type in parameterTypes)
        {
          if (!_dependencies.ContainsKey(type))
          {
            object dependency = null;

            if (mocks.ContainsKey(type))
            {
              dependency = mocks[type];
            }
            else
            {
              dependency = MockDependency(type);
            }

            _dependencies.Add(type, dependency);
          }

          parameters.Add(_dependencies[type]);
        }

        return (T)constructor.Invoke(parameters.ToArray());
      }

      return (T)Activator.CreateInstance(typeof(T));
    }
    
    public void Dispose()
    {
      _target = null;
      _dependencies.Clear();
    }
  }

  internal static class TypeUtility
  {
    public static ConstructorInfo GetConstructorWithLongestParamList(Type type)
    {
      ConstructorInfo[] publicConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

      if (!publicConstructors.Any())
      {
        throw new ArgumentException("Class has no public constructors");
      }

      return publicConstructors
        .OrderByDescending(x => x.GetParameters().Length)
        .First();
    }
  }
}
