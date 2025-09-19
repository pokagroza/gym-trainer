using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly GymDbContext _context;
        public UserController(GymDbContext context)
        {
            _context = context;
        }

        [HttpGet("by-username/{username}")]
        public ActionResult<User> GetByUsername(string username)
        {
            try
            {
                var auth = _context.UserAuths.FirstOrDefault(u => u.Username == username);
                if (auth == null) return NotFound();
                var user = _context.Users.FirstOrDefault(u => u.Id == auth.UserId);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при поиске пользователя: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при поиске пользователя по id: {ex.Message}");
            }
        }
    }
}