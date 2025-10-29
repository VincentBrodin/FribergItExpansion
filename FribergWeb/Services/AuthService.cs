using System.Net;
using Blazored.LocalStorage;
using FribergShared.Dto;
using FribergWeb.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace FribergWeb.Services;

public class AuthService(HttpClient client, ILocalStorageService localStorage, ApiAuthStateProvider authStateProvider)
{
    public async Task<string?> GetLoginTokenAsync(LoginDto loginDto)
    {
        Console.WriteLine("Trying to login");
        var res = await client.PostAsJsonAsync("", loginDto);
        if (res == null || res.StatusCode != HttpStatusCode.Accepted)
        {
            return null;
        }
        var token = res.Content.ToString();
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        Console.WriteLine($"Login token: {token}");
        await localStorage.SetItemAsync("accessToken", token);
        await authStateProvider.LoggedIn();
        return token;
    }
}
