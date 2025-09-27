using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Infrastructure.Persistence;
using System.Linq;

public class ProfileModel : PageModel
{
    private readonly GymDbContext _context;
    public new User User { get; set; } = new User();

    public ProfileModel(GymDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Login");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null) return Page();
            User = user;
            return Page();
    }
}
