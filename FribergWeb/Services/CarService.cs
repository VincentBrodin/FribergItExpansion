using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using FribergShared.Dto;
using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace FribergWeb.Services;

public class CarService(HttpClient client, ILocalStorageService localStorage)
{
    public async Task<List<FullCarDto>> GetCarsAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Cars");
        var token = await localStorage.GetItemAsync<string>("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.SendAsync(request);
        return await result.Content.ReadFromJsonAsync<List<FullCarDto>>() ?? [];
    }

    public async Task<FullCarDto?> GetCarAsync(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/Car", "id", id));
        var token = await localStorage.GetItemAsync<string>("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.SendAsync(request);
        return await result.Content.ReadFromJsonAsync<FullCarDto>();
    }

    public async Task<bool> UpdateCarAsync(string id, UpdateCarDto model)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, QueryHelpers.AddQueryString("/Car", "id", id))
        {
            Content = JsonContent.Create(model)
        };

        var token = await localStorage.GetItemAsync<string>("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        Console.WriteLine(response.StatusCode);
        return response.StatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteCarAsync(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, QueryHelpers.AddQueryString("/Car", "id", id));
        var token = await localStorage.GetItemAsync<string>("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        Console.WriteLine(response.StatusCode);
        return response.StatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> RestoreCarAsync(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, QueryHelpers.AddQueryString("/Car", "id", id));
        var token = await localStorage.GetItemAsync<string>("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        Console.WriteLine(response.StatusCode);
        return response.StatusCode == HttpStatusCode.OK;
    }
}
