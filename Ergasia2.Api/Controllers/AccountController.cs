using Ergasia2.Api.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia2.Api.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    #region DI

    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    #endregion

    [AllowAnonymous]
    [HttpPost("signup")]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Signup([FromBody] SignupBm signupBm)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(signupBm.Email);

            if (existingUser != null)
                return BadRequest(new { message = "Email is already registered." });

            var user = new IdentityUser
            {
                UserName = signupBm.Email,
                Email = signupBm.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, signupBm.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }


    [AllowAnonymous]
    [HttpPost("auth/login")]
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, UnauthorizedHttpResult>> Login([FromBody] LoginBm loginBm)
    {
        try
        {
            IdentityUser? user = await _userManager.FindByEmailAsync(loginBm.Email);

            if (user == null)
                return TypedResults.Unauthorized();

            if (await _userManager.HasPasswordAsync(user) == false)
                return TypedResults.Unauthorized();

            _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

            var result = await _signInManager.PasswordSignInAsync(
                user: user!,
                password: loginBm.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return TypedResults.Unauthorized();
            }

            return TypedResults.Empty;

        }
        catch (Exception)
        {
            throw;
        }
    }

    //this exists just because it is in the requirements
    [HttpGet("auth/logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout successful." });
    }
}
