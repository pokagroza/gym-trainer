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
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(3)]
        public string Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MinLength(4)]
        public string Password { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Range(10,120)]
        public int Age { get; set; }
        [System.ComponentModel.DataAnnotations.Range(50,250)]
        public double Height { get; set; }
        [System.ComponentModel.DataAnnotations.Range(20,400)]
        public double Weight { get; set; }
    }
    public class LoginDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        public string Password { get; set; } = string.Empty;
    }
}