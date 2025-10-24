
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
