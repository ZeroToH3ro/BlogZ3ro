namespace Blog.Models.Account;

public class ApplicationUser
{
    public int ApplicationUserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string FullName { get; set; }
}