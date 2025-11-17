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
public class CarController(UserManager<ApiUser> userManager, IRepository<Car> carRepo, IRepository<Update> updateRepo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string id)
    {
        var canViewDeleted = false;
        var user = await Auth.GetUser(HttpContext, userManager);
        if (user != null)
        {
            canViewDeleted = await userManager.IsInRoleAsync(user, ApiRoles.Admin);
        }
        var car = await carRepo.GetAsync(c => c.Id == id);
        if (car == null || (car.Deleted && !canViewDeleted))
        {
            return BadRequest($"No car with id {id}");
        }
        var users = new Dictionary<string, UserDto>(); // Keeps a per request cache of each user
        return Ok(await GetFullCarDto(car, users));
    }

    [HttpGet("/[Controller]s")]
    public async Task<IActionResult> GetAll()
    {

        var canViewDeleted = false;
        var user = await Auth.GetUser(HttpContext, userManager);
        if (user != null)
        {
            canViewDeleted = await userManager.IsInRoleAsync(user, ApiRoles.Admin);
        }
        var cars = await carRepo.AllAsync();

        var carsDto = new List<FullCarDto>();
        var users = new Dictionary<string, UserDto>(); // Keeps a per request cache of each user
        foreach (var car in cars)
        {
            if (car.Deleted && !canViewDeleted)
            {
                continue;
            }

            carsDto.Add(await GetFullCarDto(car, users));
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


        await carRepo.AddAsync(car);
        await updateRepo.AddAsync(createNote);
        await carRepo.SaveChangesAsync();
        return Ok(car?.Id);
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

        var car = await carRepo.GetAsync(c => c.Id == id);
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
            await updateRepo.AddAsync(note);
        }

        if (updateCar.Model != null)
        {
            var note = CreateNote(car, user, count, "model", car.Model, updateCar.Model);
            car.Model = updateCar.Model;
            count++;
            await updateRepo.AddAsync(note);
        }

        if (updateCar.Year != null)
        {
            var note = CreateNote(car, user, count, "year", car.Year.ToString(), (updateCar.Year ?? 0).ToString());
            car.Year = updateCar.Year ?? 0;
            count++;
            await updateRepo.AddAsync(note);
        }

        if (updateCar.HorsePower != null)
        {
            var note = CreateNote(car, user, count, "horsepower", car.HorsePower.ToString(), (updateCar.HorsePower ?? 0).ToString());
            car.HorsePower = updateCar.HorsePower ?? 0;
            count++;
            await updateRepo.AddAsync(note);
        }

        carRepo.Update(car);
        await carRepo.SaveChangesAsync();
        return Ok($"Car {id} updated");
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

        var car = await carRepo.GetAsync(c => c.Id == id);
        if (car == null || car.Deleted)
        {
            return BadRequest($"No car with id {id}");
        }

        var note = CreateNote(car, user, car.Updates.Count, "deleted", car.Deleted.ToString(), true.ToString());
        await updateRepo.AddAsync(note);
        car.Deleted = true;
        carRepo.Update(car);
        await carRepo.SaveChangesAsync();

        return Ok($"Car {id} deleted");
    }

    [HttpPut]
    [Authorize(Roles = ApiRoles.Admin)]
    public async Task<IActionResult> Restore([FromQuery] string id)
    {
        var user = await Auth.GetUser(HttpContext, userManager);
        if (user == null)
        {
            return Unauthorized("Could not find user");
        }

        var car = await carRepo.GetAsync(c => c.Id == id);
        if (car == null)
        {
            return BadRequest($"No car with id {id}");
        }

        var note = CreateNote(car, user, car.Updates.Count, "restore", car.Deleted.ToString(), false.ToString());
        await updateRepo.AddAsync(note);
        car.Deleted = false;
        carRepo.Update(car);
        await carRepo.SaveChangesAsync();

        return Ok($"Car {id} restored");
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

    private async Task<FullCarDto> GetFullCarDto(Car car, Dictionary<string, UserDto> users)
    {
        var carDto = car.ToDto();
        foreach (var update in car.Updates)
        {
            var updateDto = update.ToDto();
            updateDto.User = await GetOrFetchUserDto(update.UserId, users);
            carDto.Updates.Add(updateDto);
        }
        foreach (var rental in car.Rentals)
        {
            var rentalDto = rental.ToDto();
            rentalDto.User = await GetOrFetchUserDto(rental.UserId, users);
            carDto.Rentals.Add(rentalDto);
        }
        return carDto;
    }

    private async Task<UserDto> GetOrFetchUserDto(string id, Dictionary<string, UserDto> users)
    {
        UserDto userDto;
        if (users.TryGetValue(id, out UserDto? value) && value != null)
        {
            return value;
        }
        else
        {
            var user = await userManager.FindByIdAsync(id);
            userDto = user == null ? new UserDto { Id = id } : user.ToDto();
            users.Add(id, userDto);
            return userDto;
        }
    }
}
