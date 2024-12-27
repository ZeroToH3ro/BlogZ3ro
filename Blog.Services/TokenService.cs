using Blog.Models.Account;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;
    private readonly string _issuer;

    public TokenService()
    {
        
    }
    
    public string GenerateToken(ApplicationUserIdentity token)
    {
        
    }
}