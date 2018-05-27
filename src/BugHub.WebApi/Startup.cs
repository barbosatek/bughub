using System.Web.Http;
using Microsoft.Owin;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;

[assembly: OwinStartup(typeof(BugHub.WebApi.Startup))]
namespace BugHub.WebApi
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      HttpConfiguration config = new HttpConfiguration();
      var kernel = NinjectConfig.CreateKernel();
      
      WebApiConfig.Register(config);

      app.UseNinjectMiddleware(() => kernel)
        .UseNinjectWebApi(config);;
    }
  }
}