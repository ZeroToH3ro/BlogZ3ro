using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Account;

public class ApplicationUserCreate : ApplicationUserLogin
{
    [MinLength(10, ErrorMessage = "Must be 10-30 characters")]
    [MaxLength(30, ErrorMessage = "Must be 10-30 characters")]
    public string FullName { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(30, ErrorMessage = "Max 30 characters")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
}