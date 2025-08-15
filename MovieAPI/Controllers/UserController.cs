using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models.RequestModels;
using MovieApp.Services.Interfaces;

namespace MovieApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Signup Endpoint
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupRequest request)
        {
            var result = _userService.Signup(request);
            if (!result)
                return BadRequest(new { message = "User already exists or invalid data." });

            return Created("api/user/signup", new { message = "User registered successfully." });
        }

        // Login Endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _userService.Login(request);
            if (token == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(new { token });
        }
    }
}
