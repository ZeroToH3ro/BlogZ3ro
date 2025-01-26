using Blog.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace Blog.Repository;

public interface IAccountRepository
{
    public Task<IdentityResult> CreateAsync(ApplicationUserIdentity userIdentity, CancellationToken cancellationToken);
    public Task<ApplicationUserIdentity?> GetByUsernameAsync(string normalizedUserName, CancellationToken cancellationToken);
}