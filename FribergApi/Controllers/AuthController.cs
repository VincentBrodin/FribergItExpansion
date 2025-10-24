using Microsoft.AspNetCore.Mvc;
using FribergShared.Dto;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace FribergApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto register)
    {
        return Ok(JsonSerializer.Serialize(register));
    }
}
