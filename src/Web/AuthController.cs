using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            var user = new User { Name = dto.Name, Age = dto.Age, Height = dto.Height, Weight = dto.Weight, FatigueLevel = 0 };
            var result = _authService.Register(dto.Username, dto.Password, user);
            if (!result) return Conflict("Пользователь уже существует");
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _authService.Login(dto.Username, dto.Password);
            if (user == null) return Unauthorized();
            return Ok(user);
        }
    }

    public class RegisterDto
    {
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
    }
    public class LoginDto
    {
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    }
}