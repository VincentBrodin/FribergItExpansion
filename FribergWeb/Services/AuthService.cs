using System.Net;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using FribergShared.Dto;
using FribergWeb.Providers;

namespace FribergWeb.Services;

public class AuthService(HttpClient client, ILocalStorageService localStorage, ApiAuthStateProvider authStateProvider)
{
    public async Task<string?> GetLoginTokenAsync(LoginDto loginDto)
    {
        Console.WriteLine("Trying to login");
        var res = await client.PostAsJsonAsync("/Auth/Login", loginDto);
        if (res == null || res.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }
        var token = await res.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        Console.WriteLine($"Login token: {token}");
        await localStorage.SetItemAsync("accessToken", token);
        await authStateProvider.LoggedIn();
        return token;
    }

    public async Task Logout()
    {
        await authStateProvider.LoggedOut();
    }
}
