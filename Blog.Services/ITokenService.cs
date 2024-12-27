using Blog.Models.Account;

namespace Blog.Services;

public interface ITokenService
{
    public string GenerateToken(ApplicationUserIdentity user);
}