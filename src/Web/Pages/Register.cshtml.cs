using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;

public class RegisterModel : PageModel
{
    private readonly AuthService _authService;

    public RegisterModel(AuthService authService)
    {
        _authService = authService;
    }

    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public int Age { get; set; }
    [BindProperty] public double Height { get; set; }
    [BindProperty] public double Weight { get; set; }
    public string Message { get; set; } = string.Empty;

    public void OnGet() { }

    public IActionResult OnPost()
    {
        var user = new User { Name = Name, Age = Age, Height = Height, Weight = Weight, FatigueLevel = 0 };
        var result = _authService.Register(Username, Password, user);
        if (!result)
        {
            Message = "Ошибка регистрации: пользователь уже существует или данные некорректны.";
            return Page();
        }

        // Попробуем сразу получить пользователя и записать в сессию
        var loggedUser = _authService.Login(Username, Password);
        if (loggedUser != null)
        {
            HttpContext.Session.SetInt32("UserId", loggedUser.Id);
            return Redirect("/Profile");
        }

        // Если авто-логин не удался — отправим на страницу входа
        return Redirect("/Login");
    }
}