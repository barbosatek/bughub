using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BugHub.Data.Entities;
using BugHub.Data.Repositories;
using BugHub.WebApi.Models.V1;
using Microsoft.AspNetCore.JsonPatch;

namespace BugHub.WebApi.Controllers
{
  [RoutePrefix("api/v1/bugs")]
  public class BugsController : ApiController
  {
    private static readonly List<string> InvalidPatchableFields = new List<string> {"/id"};
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

    [HttpGet]
    [Route("{id}")]
    public async Task<IHttpActionResult> Get([FromUri] long id)
    {
      var entity = await _bugRepository.Get(id);
      if (entity == null)
      {
        return NotFound();
      }

      var model = _mapper.Map<Bug>(entity);

      return Ok(model);
    }

    [HttpDelete]
    [Route("")]
    public async Task<IHttpActionResult> Delete([FromUri] long id)
    {
      await _bugRepository.Delete(id);
      return this.NoContent();
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
    
    [HttpPatch]
    [Route("{id}")]
    public async Task<IHttpActionResult> Patch([FromBody]JsonPatchDocument<Bug> patchDocument, [FromUri]long id)
    {
      if (patchDocument.Operations.Select(x => x.path).Any(p =>
        InvalidPatchableFields.Any(f => p.Equals(f, StringComparison.InvariantCultureIgnoreCase))))
      {
        return this.Forbid();
      }

      var bugEntity = await _bugRepository.Get(id);
      if (bugEntity == null)
      {
        return NotFound();
      }
      
      var bug = _mapper.Map<Bug>(bugEntity);

      var patchErrors = new List<JsonPatchError>();
      patchDocument.ApplyTo(bug, x => { patchErrors.Add(x);});

      if (patchErrors.Any())
      {
        var msg = patchErrors.Select(x => x.ErrorMessage).ToList();
        return BadRequest(string.Join(",", msg));
      }

      var patchedBugEntity = _mapper.Map<BugEntity>(bug);
      await _bugRepository.Update(patchedBugEntity);

      return this.NoContent();
    }
  }
}
