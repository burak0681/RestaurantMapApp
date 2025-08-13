using RestaurantMapApp.Models;
using System.Text.Json;

namespace RestaurantMapApp.Services;

public interface IRestaurantService
{
    Task<List<Restaurant>> GetRestaurantsAsync();
}

public class RestaurantService : IRestaurantService
{
    public async Task<List<Restaurant>> GetRestaurantsAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("restaurants.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();
        var list = JsonSerializer.Deserialize<List<Restaurant>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return list ?? new();
    }
}