using FribergShared.Dto;

namespace FribergWeb.Services;

public class CarService(HttpClient client)
{
    public async Task<List<FullCarDto>> GetCarsAsync()
    {
        return await client.GetFromJsonAsync<List<FullCarDto>>("/Cars") ?? [];
    }
}
