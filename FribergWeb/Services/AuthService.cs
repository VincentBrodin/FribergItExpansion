using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using FribergShared.Dto;
using FribergWeb.Providers;

namespace FribergWeb.Services;

public class AuthService(HttpClient client, ILocalStorageService localStorage, ApiAuthStateProvider authStateProvider)
{

    public async Task<FullUserDto?> FetchAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Auth/Fetch");
        var token = await authStateProvider.GetOrRefreshToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.SendAsync(request);
        return await result.Content.ReadFromJsonAsync<FullUserDto>();
    }

    public async Task<bool> DeleteAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/Auth/Delete");
        var token = await authStateProvider.GetOrRefreshToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.SendAsync(request);
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterAsync(RegisterDto registerDto)
    {
        Console.WriteLine("Trying to register");
        var res = await client.PostAsJsonAsync("/Auth/Register", registerDto);
        if (res == null || !res.IsSuccessStatusCode)
        {
            return false;
        }
        var tokens = await res.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (tokens == null)
        {
            return false;
        }

        await localStorage.SetItemAsync("access_token", tokens.AccessToken);
        await localStorage.SetItemAsync("refresh_token", tokens.RefreshToken);
        await authStateProvider.LoggedIn();
        return true;
    }

    public async Task<bool> LoginAsync(LoginDto loginDto)
    {
        Console.WriteLine("Trying to login");
        var res = await client.PostAsJsonAsync("/Auth/Login", loginDto);
        if (res == null || !res.IsSuccessStatusCode)
        {
            return false;
        }
        var tokens = await res.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (tokens == null)
        {
            return false;
        }

        await localStorage.SetItemAsync("access_token", tokens.AccessToken);
        await localStorage.SetItemAsync("refresh_token", tokens.RefreshToken);
        await authStateProvider.LoggedIn();
        return true;
    }

    public async Task Logout()
    {
        await authStateProvider.LoggedOut();
    }
}
