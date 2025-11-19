using FribergApi.Data;
using FribergApi.Models;
using FribergApi.Tools;
using FribergShared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FribergApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class RentalController(UserManager<ApiUser> userManager, IRepository<Rental> rentalRepo, IRepository<Car> carRepo) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RentCar([FromQuery] string id, [FromBody] RentCarDto rentCar)
    {
        var car = await carRepo.GetAsync(c => c.Id == id);
        if (car == null)
        {
            return BadRequest($"No car with id {id}");
        }

        var user = await Auth.GetUser(HttpContext, userManager);
        if (user == null)
        {
            return Unauthorized("Could not find user");
        }



        var rental = new Rental
        {
            Id = Guid.NewGuid().ToString(),
            CarId = car.Id,
            UserId = user.Id,
            Start = rentCar.Start,
            End = rentCar.End,
        };

        await rentalRepo.AddAsync(rental);
        await rentalRepo.SaveChangesAsync();
        return Ok(GetFullRentalDto(rental));
    }


    private async Task<FullRentalDto> GetFullRentalDto(Rental rental)
    {
        var dto = rental.ToDto();
        dto.Car = (await carRepo.GetAsync(c => c.Id == rental.CarId) ?? new Car() { Id = rental.Id }).ToDto();
        dto.User = (await userManager.FindByIdAsync(rental.UserId) ?? new ApiUser()).ToDto();
        return dto;
    }
}
