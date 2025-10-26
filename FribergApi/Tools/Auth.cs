using System.Security.Claims;
using FribergApi.Models;
using Microsoft.AspNetCore.Identity;

namespace FribergApi.Tools;

public static class Auth
{
    public static async Task<ApiUser?> GetUser(HttpContext httpContext, UserManager<ApiUser> userManager)
    {
        var uid = httpContext.User.FindFirstValue("uid");
        if (uid == null)
        {
            return null;
        }
        return await userManager.FindByIdAsync(uid);
    }

}
