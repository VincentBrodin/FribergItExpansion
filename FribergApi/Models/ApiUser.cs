using Microsoft.AspNetCore.Identity;

namespace FribergApi.Models;

public class ApiUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
