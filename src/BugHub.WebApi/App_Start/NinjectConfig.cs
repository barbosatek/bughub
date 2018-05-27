using System;
using System.Web;
using System.Web.Configuration;
using AutoMapper;
using BugHub.Data.Context;
using BugHub.Data.Entities;
using BugHub.Data.Repositories;
using BugHub.WebApi.Models.V1;
using Ninject;
using Ninject.Web.Common;

namespace BugHub.WebApi
{
  public static class NinjectConfig
  {
    internal static IKernel CreateKernel()
    {
      var kernel = new StandardKernel();
      try
      {
        kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
        kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

        RegisterServices(kernel);
        return kernel;
      }
      catch
      {
        kernel.Dispose();
        throw;
      }
    }

    public static void RegisterServices(IKernel kernel)
    {
      kernel.Bind<IMapper>().ToMethod(k =>
      {
        Mapper.Initialize(cfg => {
          cfg.CreateMap<Bug, BugEntity>(MemberList.Source);
          cfg.CreateMap<BugEntity, Bug>(MemberList.Source);
        });

        return Mapper.Instance;
      }).InSingletonScope();

      kernel.Bind<IBugRepository>().To<BugRepository>().InSingletonScope();
      kernel.Bind<IDbContextFactory>().ToMethod(k =>
      {
        var connectionOptions = new SqlConnectionOptions
        {
          DataSource = WebConfigurationManager.AppSettings["SqlConnection.DataSource"],
          InitialCatalog = WebConfigurationManager.AppSettings["SqlConnection.InitialCatalog"],
          UseWindowsAuthentication = true
        };

        return new DbContextFactory(connectionOptions);
      }).InSingletonScope();
    }
  }
}