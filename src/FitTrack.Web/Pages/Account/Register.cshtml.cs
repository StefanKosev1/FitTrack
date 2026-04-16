using FitTrack.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitTrack.Web.Pages.Account;

public class RegisterModel : PageModel
{
    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public RegisterInputModel Input { get; set; } = new();

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
        StatusMessage = $"Demo account created for {Input.FullName}. Persistence is not wired yet.";
        return RedirectToPage("/Index");
    }
}
