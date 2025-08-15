using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models.RequestModels;
using MovieApp.Services.Interfaces;
using MovieApp.Exceptions;

namespace MovieApp.Controllers
{
    [Authorize]
    [Route("api/actors")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var actors = _actorService.GetAll();
            return Ok(actors);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var actor = _actorService.GetById(id);
                return Ok(actor);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] ActorRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newActorId = _actorService.Add(request);
            return CreatedAtAction(nameof(GetById), new { id = newActorId }, new { id = newActorId });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ActorRequest request)
        {
            try
            {
                var updated = _actorService.Update(id, request);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var deleted = _actorService.Delete(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
