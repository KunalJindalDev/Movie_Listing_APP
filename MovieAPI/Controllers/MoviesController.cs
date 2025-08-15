using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Helpers;
using MovieApp.Models.RequestModels;
using MovieApp.Services.Interfaces;
using MovieApp.Exceptions;
using MovieApp.Services;

namespace MovieApp.Controllers
{
    [Authorize]
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int? year)
        {
            try
            {
                var movies = _movieService.GetAll(year);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var movie = _movieService.GetById(id);
                return Ok(movie);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] MovieRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newMovieId = _movieService.Add(request);
                return CreatedAtAction(nameof(GetById), new { id = newMovieId }, new { id = newMovieId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] MovieRequest request)
        {
            try
            {
                _movieService.Update(id, request);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _movieService.Delete(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("upload-poster")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPoster([FromForm] IFormFile file, [FromServices] SupabaseUploader uploader)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            try
            {
                using var stream = file.OpenReadStream();
                var url = await uploader.UploadPosterAsync(stream, fileName, file.ContentType);
                return Ok(new { Url = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Upload failed: {ex.Message}");
            }
        }
    }
}
