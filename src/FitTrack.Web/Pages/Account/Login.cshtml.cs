using FitTrack.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitTrack.Web.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
