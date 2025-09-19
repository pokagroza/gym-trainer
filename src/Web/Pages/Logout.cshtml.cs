using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // Clear session and redirect to home. Use GET so simple anchor links work without antiforgery.
        HttpContext.Session.Clear();
        return RedirectToPage("/Index");
    }
}
