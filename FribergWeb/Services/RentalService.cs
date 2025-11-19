using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using FribergShared.Dto;
using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace FribergWeb.Services;

public class RentalService(HttpClient client, ILocalStorageService localStorage)
{
    public async Task<FullRentalDto?> RentCarAsync(string id, RentCarDto model)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, QueryHelpers.AddQueryString("/Rental", "id", id))
        {
            Content = JsonContent.Create(model)
        };
        var token = await localStorage.GetItemAsync<string>("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.SendAsync(request);
        if(!result.IsSuccessStatusCode) {
            return null;
        }
        return await result.Content.ReadFromJsonAsync<FullRentalDto>();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, QueryHelpers.AddQueryString("/Rental", "id", id));
        var token = await localStorage.GetItemAsync<string>("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.SendAsync(request);
        return result.IsSuccessStatusCode;
    }
}
