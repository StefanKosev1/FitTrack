using FitTrack.Core.Interfaces.Services;
using FitTrack.Core.Results;
using FitTrack.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FitTrack.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IRegistrationService registrationService, ILogger<RegisterModel> logger)
    {
        _registrationService = registrationService;
        _logger = logger;
    }

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

        AuthResult result;
        try
        {
            result = await _registrationService.RegisterAsync(Input.FullName, Input.Email, Input.Password);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Registration failed because the user repository is unavailable.");
            ModelState.AddModelError(string.Empty, "Account creation is temporarily unavailable. Check the database connection and try again.");
            return Page();
        }

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Registration failed.");
            return Page();
        }

        await SignInUserAsync(result.UserId!.Value, result.FullName!, result.Email!);
        StatusMessage = $"Account created for {result.FullName}.";
        return RedirectToPage("/Index");
    }

    private async Task SignInUserAsync(Guid userId, string fullName, string email)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, fullName),
            new(ClaimTypes.Email, email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true
            });
    }
}
