using Microsoft.AspNetCore.Mvc;
using FribergShared.Dto;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using FribergApi.Data;
using FribergShared.Constants;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace FribergApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class AuthController(UserManager<ApiUser> userManager, IConfiguration configuration) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto register)
    {
        var user = new ApiUser()
        {
            FirstName = register.FirstName,
            LastName = register.LastName,
            UserName = register.Email,
            Email = register.Email,
        };

        var res = await userManager.CreateAsync(user, register.Password);

        if (res == null)
        {
            return BadRequest();
        }
        else if (!res.Succeeded)
        {
            return BadRequest(res.Errors);
        }

        await userManager.AddToRoleAsync(user, ApiRoles.User);

        string token = await GenerateToken(user);
        return Ok(token);
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var user = await userManager.FindByEmailAsync(login.UserName);
        if (user == null)
        {
            return BadRequest("UserName or Password is wrong");
        }

        var validPassword = await userManager.CheckPasswordAsync(user, login.Password);
        if (!validPassword)
        {
            return BadRequest("UserName or Password is wrong");
        }

        string token = await GenerateToken(user);
        return Ok(token);
    }

    private async Task<string> GenerateToken(ApiUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"] ?? throw new NullReferenceException("Missing jwt secret key")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);
        var userClaims = await userManager.GetClaimsAsync(user);

        var claims = new List<Claim> {
            new("uid", user.Id),
            new(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }.Union([.. roles.Select(r => new Claim(ClaimTypes.Role, r))])
        .Union(userClaims);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
