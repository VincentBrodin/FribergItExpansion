using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text.Json;
using FribergApi.Models;
using FribergShared.Constants;
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

    public static async Task<bool> IsAdmin(HttpContext httpContext, UserManager<ApiUser> userManager)
    {
        var user = await GetUser(httpContext, userManager);
        if (user == null)
        {
            return false;
        }
        var roles = await userManager.GetRolesAsync(user);
        if (roles == null)
        {
            return false;
        }
        return roles.Contains(ApiRoles.Admin);
    }

}
