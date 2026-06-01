using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitTrack.Web.Pages.Account;

public class LogoutModel : PageModel
{
    [TempData]
    public string? StatusMessage { get; set; }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        StatusMessage = "You have been signed out.";
        return RedirectToPage("/Index");
    }
}
