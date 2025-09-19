using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;

public class LoginModel : PageModel
{
    private readonly AuthService _authService;

    public LoginModel(AuthService authService)
    {
        _authService = authService;
    }

    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public void OnGet() { }

    public IActionResult OnPost()
    {
        var user = _authService.Login(Username, Password);
        if (user != null)
        {
            HttpContext.Session.SetInt32("UserId", user.Id);
            return Redirect("/Profile");
        }

        Message = "Ошибка входа";
        return Page();
    }
}