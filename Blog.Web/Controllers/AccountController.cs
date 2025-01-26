using Blog.Models.Account;
using Blog.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUserIdentity> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<ApplicationUserIdentity> _signInManager;

    public AccountController(
        ITokenService tokenService,
        UserManager<ApplicationUserIdentity> userManager,
        SignInManager<ApplicationUserIdentity> signInManager
        )
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
    }
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
                    Token = _tokenService.GenerateToken(applicationUserIdentity)
                };

                return Ok(applicationUser);
            }
        }
        
        
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApplicationUser>> Login(ApplicationUserLogin model)
    {
        var applicationUserIdentity = await _userManager.FindByNameAsync(model.Username);

        if (applicationUserIdentity == null)
        {
            return BadRequest("Can not find user name");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            applicationUserIdentity,
            model.Password, false
        );

        if (result.Succeeded)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                ApplicationUserId = applicationUserIdentity.ApplicationUserId,
                UserName = applicationUserIdentity.Username,
                Email = applicationUserIdentity.Email,
                FullName = applicationUserIdentity.FullName,
                Token = _tokenService.GenerateToken(applicationUserIdentity)
            };
            
            return Ok(applicationUser);
        }
        
        return BadRequest("Invalid Login Attempt");
    }
}