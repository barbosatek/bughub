using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BugHub.Data.Repositories;
using BugHub.WebApi.Models.V1;

namespace BugHub.WebApi.Controllers
{
  [RoutePrefix("api/v1/bugs")]
  public class BugsController : ApiController
  {
    private readonly IBugRepository _bugRepository;

    public BugsController(IBugRepository bugRepository)
    {
      _bugRepository = bugRepository;
    }

    [HttpGet]
    [Route("")]
    public async Task<IHttpActionResult> Get()
    {
      var entities = await _bugRepository.Get();
      var models = entities.Select(Mapper.Map<Bug>).ToList();

      return Ok(models);
    }
  }
}
