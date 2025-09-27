using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using System.ComponentModel.DataAnnotations;
using Domain.Entities;

public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty, Required, StringLength(50, ErrorMessage = "Максимум 50 символов")]
    public string Username { get; set; } = string.Empty;

    [BindProperty, Required, StringLength(100, ErrorMessage = "Максимум 100 символов")]
    public string Password { get; set; } = string.Empty;

    [BindProperty, Required, StringLength(80, ErrorMessage = "Максимум 80 символов")]
    public string Name { get; set; } = string.Empty;

    [BindProperty, Range(12, 90, ErrorMessage = "Возраст от 12 до 90")]
    public int Age { get; set; }

    [BindProperty, Range(100, 250, ErrorMessage = "Рост от 100 до 250 см")]
    public double Height { get; set; }

    [BindProperty, Range(30, 300, ErrorMessage = "Вес от 30 до 300 кг")]
    public double Weight { get; set; }
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
        Name = Name.Trim();
        var user = new User { Name = Name, Age = Age, Height = Height, Weight = Weight, FatigueLevel = 0 };
        var result = _authService.Register(Username, Password, user);
        if (!result)
        {
            Message = "Ошибка: пользователь уже существует.";
            return Page();
        }

        var loggedUser = _authService.Login(Username, Password);
        if (loggedUser != null)
        {
            HttpContext.Session.SetInt32("UserId", loggedUser.Id);
            return Redirect("/Profile");
        }
        return Redirect("/Login");
    }
}