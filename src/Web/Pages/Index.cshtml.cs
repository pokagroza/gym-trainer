using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Entities;
using System.Collections.Generic;

public class IndexModel : PageModel
{
    public TrainingSplit Split { get; set; } = null!;
    public void OnGet()
    {
        // Пусто — данные подгружаются через форму
    }
}
