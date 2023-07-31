using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestCaseDotnet.Dtos;
using TestCaseDotnet.Entities;
using TestCaseDotnet.Services;

namespace TestCaseDotnet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly TokenService _tokenService;

    public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    private async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }


    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
            return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (result.Succeeded)
            return new UserDto
            {
                Email = user.Email!,
                Token = _tokenService.GenerateJwtToken(user.Email)
            };    

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (CheckEmailExistsAsync(registerDto.Email).Result)
        {
            return new BadRequestObjectResult(new[] { "Email address is in use" } );
        }

        var user = new User
        {
            
            Email = registerDto.Email,
            UserName = registerDto.Email,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            return new UserDto
            {
                Token = _tokenService.GenerateJwtToken(user.Email),
                Email = user.Email
            };
        }

        return BadRequest();
    }
}
