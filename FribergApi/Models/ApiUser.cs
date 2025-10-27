using FribergShared.Dto;
using Microsoft.AspNetCore.Identity;

namespace FribergApi.Models;

public class ApiUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public UserDto ToDto()
    {
        return new UserDto
        {
            Id = Id,
            UserName = UserName ?? string.Empty,
            FirstName = FirstName,
            LastName = LastName,
        };

    }
}
