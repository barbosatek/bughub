using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BugHub.Data.Entities;
using BugHub.Data.Repositories;
using BugHub.WebApi.Models.V1;

namespace BugHub.WebApi.Controllers
{
  [RoutePrefix("api/v1/bugs")]
  public class BugsController : ApiController
  {
    private readonly IBugRepository _bugRepository;
    private readonly IMapper _mapper;

    public BugsController(IBugRepository bugRepository, IMapper mapper)
    {
      _bugRepository = bugRepository;
      _mapper = mapper;
    }

    [HttpGet]
    [Route("")]
    public async Task<IHttpActionResult> Get()
    {
      var entities = await _bugRepository.Get();
      var models = entities.Select(_mapper.Map<Bug>).ToList();

      return Ok(models);
    }

    [HttpPut]
    [Route("")]
    public async Task<IHttpActionResult> Put([FromBody] Bug bug)
    {
      var bugEntity = _mapper.Map<BugEntity>(bug);
      bugEntity = await _bugRepository.Create(bugEntity);
      bug = _mapper.Map<Bug>(bugEntity);

      var locationUri = $"{Request.RequestUri}/{bug.Id}";
      return Created<Bug>(locationUri, bug);
    }
  }
}
