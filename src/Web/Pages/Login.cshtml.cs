using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using System.ComponentModel.DataAnnotations;

public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty, Required(ErrorMessage = "Введите логин"), StringLength(50, ErrorMessage = "Максимум 50 символов")]
    public string Username { get; set; } = string.Empty;

    [BindProperty, Required(ErrorMessage = "Введите пароль"), StringLength(100, ErrorMessage = "Максимум 100 символов")]
    public string Password { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            Message = "Исправьте ошибки формы.";
            return Page();
        }

        Username = Username.Trim();
        var user = _authService.Login(Username, Password);
        if (user != null)
        {
            HttpContext.Session.SetInt32("UserId", user.Id);
            return Redirect("/Profile");
        }
        Message = "Ошибка входа: неверный логин или пароль.";
        return Page();
    }
}