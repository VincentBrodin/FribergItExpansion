using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using FribergShared.Dto;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace FribergWeb.Providers;

public class ApiAuthStateProvider(HttpClient client, ILocalStorageService localStorage, JwtSecurityTokenHandler jwtSecurity) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var savedToken = await GetOrRefreshToken();

        if (savedToken == null)
        {
            Console.WriteLine("Nothing saved to storage");
            return new AuthenticationState(user);
        }

        var tokenContent = jwtSecurity.ReadJwtToken(savedToken);
        if (tokenContent.ValidTo < DateTime.UtcNow)
        {
            Console.WriteLine("OLD!");
            return new AuthenticationState(user);
        }

        var claims = await GetClaimsAsync();
        user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        return new AuthenticationState(user);
    }

    public async Task LoggedIn()
    {
        var claims = await GetClaimsAsync();
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task LoggedOut()
    {
        await localStorage.RemoveItemAsync("access_token");
        var nobody = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(nobody));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task<string?> GetOrRefreshToken()
    {

        var savedAccessToken = await localStorage.GetItemAsync<string>("access_token");
        if(savedAccessToken == null) {
            return null;
        }
        var tokenContent = jwtSecurity.ReadJwtToken(savedAccessToken);
        if(tokenContent == null) {
            return null;
        }

        if (tokenContent.ValidTo < DateTime.UtcNow.AddSeconds(30)) // If true then old
        {
            Console.WriteLine("REFRESHING TOKENS");
            var savedRefreshToken = await localStorage.GetItemAsync<string>("refresh_token");
            var request = new HttpRequestMessage(HttpMethod.Post, "/Auth/Refresh")
            {

                Content = JsonContent.Create(new RefreshRequestDto
                {
                    RefreshToken = savedRefreshToken ?? string.Empty,
                })
            };
            var result = await client.SendAsync(request);
            var tokens = await result.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (tokens == null)
            {
                return null;
            }
            else
            {
                await localStorage.SetItemAsync("access_token", tokens.AccessToken);
                await localStorage.SetItemAsync("refresh_token", tokens.RefreshToken);
                await LoggedIn();
                return tokens.AccessToken;
            }
        }
        else
        {
            return savedAccessToken;
        }
    }

    private async Task<List<Claim>> GetClaimsAsync()
    {
        var savedToken = await localStorage.GetItemAsync<string>("access_token");
        var tokenContent = jwtSecurity.ReadJwtToken(savedToken);
        var claims = tokenContent.Claims.ToList();
        claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
        return claims;
    }
}
