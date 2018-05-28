using System;
using System.Net.Http;
using System.Web.Http;

namespace BugHub.UnitTests.Controllers
{
  public static class ApiControllerTestExtensions
  {
    public static void SetupRequest<T>(this T controller, HttpMethod httpMethod, Uri uri) where T : ApiController
    {
      controller.Request = new HttpRequestMessage(httpMethod, uri);
    }
  }
}