using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FribergWeb.Providers;

public class ApiAuthStateProvider(ILocalStorageService localStorage, JwtSecurityTokenHandler jwtSecurity) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var savedToken = await localStorage.GetItemAsync<string>("accessToken");

        if (savedToken == null)
        {
            return new AuthenticationState(user);
        }

        var tokenContent = jwtSecurity.ReadJwtToken(savedToken);
        if (tokenContent.ValidTo < DateTime.Now)
        {
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
        await localStorage.RemoveItemAsync("accessToken");
        var nobody = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(nobody));
        NotifyAuthenticationStateChanged(authState);
    }

    private async Task<List<Claim>> GetClaimsAsync()
    {
        var savedToken = await localStorage.GetItemAsync<string>("accessToken");
        var tokenContent = jwtSecurity.ReadJwtToken(savedToken);
        var claims = tokenContent.Claims.ToList();
        claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
        return claims;
    }
}
