using FribergApi.Data;
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
public class AdminController(UserManager<ApiUser> userManager, IRepository<Rental> rentalRepo, IRepository<Car> carRepo) : ControllerBase
{
    [HttpGet("Users")]
    [Authorize(Roles = ApiRoles.Admin)]
    public async Task<IActionResult> Users()
    {
        var users = Auth.GetUsers(HttpContext, userManager);
        var fullUsers = new List<FullUserDto>();
        foreach (var user in users)
        {
            var fullUser = user.ToFullDto();
            var rentals = await rentalRepo.FindAsync(r => r.UserId == user.Id);
            foreach (var rental in rentals)
            {
                fullUser.Rentals.Add(await GetFullRentalDto(rental));
            }
            fullUsers.Add(fullUser);
        }
        return Ok(fullUsers);
    }

    private async Task<FullRentalDto> GetFullRentalDto(Rental rental)
    {
        var dto = rental.ToFullDto();
        dto.Car = (await carRepo.GetAsync(c => c.Id == rental.CarId) ?? new Car() { Id = rental.Id }).ToDto();
        dto.User = (await userManager.FindByIdAsync(rental.UserId) ?? new ApiUser()).ToDto();
        return dto;
    }
}
