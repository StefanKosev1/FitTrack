using System.ComponentModel.DataAnnotations;

namespace FitTrack.Web.ViewModels.Account;

public class RegisterInputModel
{
    [Required]
    [StringLength(80)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
