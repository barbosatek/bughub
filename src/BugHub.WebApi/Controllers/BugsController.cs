using System.Web.Http;

namespace BugHub.WebApi.Controllers
{
  [RoutePrefix("api/v1/bugs")]
  public class BugsController : ApiController
  {
    [HttpGet]
    [Route("")]
    public IHttpActionResult  Get()
    {
      return Ok(new string[] {"value1", "value2"});
    }
  }
}
