using FribergApi.Data;
using FribergApi.Models;
using FribergApi.Tools;
using FribergShared.Constants;
using FribergShared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class CarController(UserManager<ApiUser> userManager, ApiContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string id)
    {
        var car = await context.Cars.Include(c => c.Updates).FirstOrDefaultAsync(c => c.Id == id);
        if (car == null || car.Deleted)
        {
            return BadRequest($"No car with id {id}");
        }
        var carDto = car.ToDto();
        foreach (var update in car.Updates)
        {
            var user = await userManager.FindByIdAsync(update.UserId);
            var userDto = user == null ? new UserDto { Id = update.Id } : user.ToDto();
            var updateDto = update.ToDto();
            updateDto.User = userDto;
            carDto.Updates.Add(updateDto);
        }
        return Ok(carDto);
    }

    [HttpGet("/[Controller]s")]
    public async Task<IActionResult> GetAll()
    {
        var cars = await context.Cars.Include(c => c.Updates).ToListAsync();
        var carsDto = new List<FullCarDto>();
        foreach (var car in cars)
        {
            if (car.Deleted)
            {
                continue;
            }

            var carDto = car.ToDto();
            foreach (var update in car.Updates)
            {
                var user = await userManager.FindByIdAsync(update.UserId);
                var userDto = user == null ? new UserDto { Id = update.Id } : user.ToDto();
                var updateDto = update.ToDto();
                updateDto.User = userDto;
                carDto.Updates.Add(updateDto);
            }
            carsDto.Add(carDto);
        }
        return Ok(carsDto);
    }

    [HttpPost]
    [Authorize(Roles = ApiRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateCarDto createCar)
    {
        var user = await Auth.GetUser(HttpContext, userManager);
        if (user == null)
        {
            return Unauthorized("Could not find user");
        }

        var carId = Guid.NewGuid().ToString();

        var car = new Car
        {
            Id = carId,
            Make = createCar.Make,
            Model = createCar.Model,
            Year = createCar.Year,
            HorsePower = createCar.HorsePower,
        };

        var createNote = new Update
        {
            Id = Guid.NewGuid().ToString(),
            UserId = user.Id,
            CarId = carId,
            Index = 0,
            TimeStamp = DateTime.Now,
            Key = "created",
        };


        await context.Cars.AddAsync(car);
        await context.Updates.AddAsync(createNote);
        await context.SaveChangesAsync();

        return Ok(car);
    }


    [HttpPatch]
    [Authorize(Roles = ApiRoles.Admin)]
    public async Task<IActionResult> Update([FromQuery] string id, [FromBody] UpdateCarDto updateCar)
    {
        var user = await Auth.GetUser(HttpContext, userManager);
        if (user == null)
        {
            return Unauthorized("Could not find user");
        }

        var car = await context.Cars.Include(c => c.Updates).FirstOrDefaultAsync(c => c.Id == id);
        if (car == null)
        {
            return BadRequest($"No car with id {id}");
        }

        int count = car.Updates.Count;

        if (updateCar.Make != null)
        {
            var note = CreateNote(car, user, count, "make", car.Make, updateCar.Make);
            car.Make = updateCar.Make;
            count++;
            await context.Updates.AddAsync(note);
        }

        if (updateCar.Model != null)
        {
            var note = CreateNote(car, user, count, "model", car.Model, updateCar.Model);
            car.Model = updateCar.Model;
            count++;
            await context.Updates.AddAsync(note);
        }

        if (updateCar.Year != null)
        {
            var note = CreateNote(car, user, count, "year", car.Year.ToString(), (updateCar.Year ?? 0).ToString());
            car.Year = updateCar.Year ?? 0;
            count++;
            await context.Updates.AddAsync(note);
        }

        if (updateCar.HorsePower != null)
        {
            var note = CreateNote(car, user, count, "horsepower", car.HorsePower.ToString(), (updateCar.HorsePower ?? 0).ToString());
            car.HorsePower = updateCar.HorsePower ?? 0;
            count++;
            await context.Updates.AddAsync(note);
        }

        context.Cars.Update(car);
        await context.SaveChangesAsync();

        return Ok(car);
    }

    [HttpDelete]
    [Authorize(Roles = ApiRoles.Admin)]
    public async Task<IActionResult> Delete([FromQuery] string id)
    {
        var user = await Auth.GetUser(HttpContext, userManager);
        if (user == null)
        {
            return Unauthorized("Could not find user");
        }

        var car = await context.Cars.Include(c => c.Updates).FirstOrDefaultAsync(c => c.Id == id);
        if (car == null || car.Deleted)
        {
            return BadRequest($"No car with id {id}");
        }

        var note = CreateNote(car, user, car.Updates.Count, "deleted", car.Deleted.ToString(), true.ToString());
        await context.Updates.AddAsync(note);
        car.Deleted = true;
        context.Cars.Update(car);
        await context.SaveChangesAsync();

        return Ok($"Car {id} deleted");
    }

    private static Update CreateNote(Car car, ApiUser user, int index, string key, string oldValue, string newValue)
    {
        return new Update
        {
            Id = Guid.NewGuid().ToString(),
            UserId = user.Id,
            CarId = car.Id,
            Index = index,
            TimeStamp = DateTime.Now,
            Key = key,
            NewValue = newValue,
            OldValue = oldValue,
        };
    }
}
