using FribergApi.Models;
using FribergApi.Tools;
using FribergShared.Constants;
using FribergShared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FribergApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class CarController(UserManager<ApiUser> userManager) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = ApiRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateCarDto createCar)
    {
        var user = await Auth.GetUser(HttpContext, userManager);
        if (user == null)
        {
            return Unauthorized("Could not find user");
        }
        return Ok(user.UserName);
    }
}
