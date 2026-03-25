using FitTrack.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitTrack.Web.Pages.Account;

public class RegisterModel : PageModel
{
    public RegisterInputModel Input { get; set; } = new();

    public void OnGet()
    {
    }
}
