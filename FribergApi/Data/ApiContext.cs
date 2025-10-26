
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FribergApi.Models;

namespace FribergApi.Data;

public class ApiContext : IdentityDbContext<ApiUser>
{
    public ApiContext()
    {
    }

    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
    }

}
