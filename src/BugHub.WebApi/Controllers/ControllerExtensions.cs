using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace BugHub.WebApi.Controllers
{
  public static class ControllerExtensions
  {
    public static IHttpActionResult NoContent(this ApiController controller)
    {
      var response = controller.Request.CreateResponse(HttpStatusCode.NoContent);
      return new ResponseMessageResult(response);
    }

    public static IHttpActionResult Forbid(this ApiController controller)
    {
      var response = controller.Request.CreateResponse(HttpStatusCode.Forbidden);
      return new ResponseMessageResult(response);
    }
  }
}