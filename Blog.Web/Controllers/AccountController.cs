using Blog.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUserIdentity> _userManager;
    
    [HttpPost("register")]
    public async Task<ActionResult<ApplicationUser>> Register(ApplicationUserCreate model)
    {
        var applicationUserIdentity = new ApplicationUserIdentity
        {
            Email = model.Email,
            Username = model.Username,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(applicationUserIdentity, model.Password);
        if (result.Succeeded)
        {
            applicationUserIdentity = await _userManager.FindByNameAsync(model.Username);
            if (applicationUserIdentity != null)
            {
                ApplicationUser applicationUser = new ApplicationUser
                {
                    ApplicationUserId = applicationUserIdentity.ApplicationUserId,
                    UserName = applicationUserIdentity.Username,
                    Email = applicationUserIdentity.Email,
                    FullName = applicationUserIdentity.FullName,
                };

                return Ok(applicationUser);
            }
        }
        
        
        return BadRequest(result.Errors);
    }
}