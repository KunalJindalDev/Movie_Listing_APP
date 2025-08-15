using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Exceptions;
using MovieApp.Models.RequestModels;
using MovieApp.Services.Interfaces;

namespace MovieApp.Controllers
{
    [Authorize]
    [Route("api/movies/{movieId}/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public IActionResult GetAll(int movieId)
        {
            try
            {
                var reviews = _reviewService.GetAll();
                var movieReviews = reviews.Where(r => r.MovieId == movieId).ToList();
                return Ok(movieReviews);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int movieId, int id)
        {
            try
            {
                var review = _reviewService.GetById(id);
                return Ok(review);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create(int movieId, [FromBody] ReviewRequest request)
        {
            try
            {
                request.MovieId = movieId; 
                var newReviewId = _reviewService.Add(request);
                return CreatedAtAction(nameof(GetById), new { movieId = movieId, id = newReviewId }, new { id = newReviewId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int movieId, int id, [FromBody] ReviewRequest request)
        {
            try
            {
                request.MovieId = movieId;
                _reviewService.Update(id, request);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int movieId, int id)
        {
            try
            {
                _reviewService.Delete(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
