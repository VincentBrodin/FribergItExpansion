using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using FribergShared.Dto;
using FribergWeb.Providers;
using Microsoft.AspNetCore.WebUtilities;

namespace FribergWeb.Services;

public class AdminService(HttpClient client, ApiAuthStateProvider authStateProvider)
{
    public async Task<List<FullUserDto>> UsersAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Admin/Users");
        var token = await authStateProvider.GetOrRefreshToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.SendAsync(request);
        return await result.Content.ReadFromJsonAsync<List<FullUserDto>>() ?? [];
    }


    public async Task<bool> ToggleAdminAsync(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/Admin/ToggleAdmin", "id", id));
        var token = await authStateProvider.GetOrRefreshToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.SendAsync(request);
        return result.IsSuccessStatusCode;
    }
}
