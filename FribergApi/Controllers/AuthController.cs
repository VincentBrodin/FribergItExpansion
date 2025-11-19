using Microsoft.AspNetCore.Mvc;
using FribergShared.Dto;
using Microsoft.AspNetCore.Identity;
using FribergApi.Models;
using FribergShared.Constants;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using FribergApi.Tools;
using Microsoft.AspNetCore.Authorization;
using FribergApi.Data;

namespace FribergApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class AuthController(UserManager<ApiUser> userManager, IConfiguration configuration, IRepository<Rental> rentalRepo, IRepository<Car> carRepo) : ControllerBase
{
    private static readonly List<string> KnownAdmins = new() {
       "vincent.brodin21@gmail.com",
    };

    [HttpGet("Fetch")]
    [Authorize]
    public async Task<IActionResult> Fetch()
    {
        var user = await Auth.GetUser(HttpContext, userManager);
        if (user == null)
        {
            return Unauthorized();
        }

        var userDto = user.ToDto();
        var fullUserDto = user.ToFullDto();
        var rentals = await rentalRepo.FindAsync(r => r.UserId == user.Id);
        foreach (var rental in rentals)
        {
            fullUserDto.Rentals.Add(await GetFullRentalDto(rental));
        }
        return Ok(fullUserDto);
    }

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

        if (KnownAdmins.Contains(user.Email))
        {
            await userManager.AddToRoleAsync(user, ApiRoles.Admin);
        }
        else
        {
            await userManager.AddToRoleAsync(user, ApiRoles.User);
        }

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

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete()
    {

        var user = await Auth.GetUser(HttpContext, userManager);
        if (user == null)
        {
            return Unauthorized("Only signed in users can delete their accounts");
        }

        var res = await userManager.DeleteAsync(user);

        if (res == null)
        {
            return BadRequest();
        }
        else if (!res.Succeeded)
        {
            return BadRequest(res.Errors);
        }

        return Ok("User deleted");
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

        var duration = Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"]);
        Console.WriteLine(duration);
        Console.WriteLine(DateTime.UtcNow.AddMinutes(duration));
        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(duration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    private async Task<FullRentalDto> GetFullRentalDto(Rental rental)
    {
        var dto = rental.ToDto();
        dto.Car = (await carRepo.GetAsync(c => c.Id == rental.CarId) ?? new Car() { Id = rental.Id }).ToDto();
        dto.User = (await userManager.FindByIdAsync(rental.UserId) ?? new ApiUser()).ToDto();
        return dto;
    }
}
