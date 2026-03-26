using System.Net.Http.Json;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthenticationService _authService;

    private const string BaseUrl = "https://set09102-api.b-davison.workers.dev";

    public ApiService(IAuthenticationService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    // Called before each authenticated request to ensure token is current
    private void EnsureAuthHeader()
    {
        var token = _authService.Token;
        if (token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        EnsureAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<ItemsResponse>("/items");
        return response?.Items ?? new List<Item>();
    }

    public async Task<Item?> GetItemAsync(int id)
    {
        EnsureAuthHeader();
        return await _httpClient.GetFromJsonAsync<Item>($"/items/{id}");
    }

    public async Task<Item?> CreateItemAsync(Item item)
    {
        EnsureAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("/items", new
        {
            title = item.Title,
            description = item.Description,
            dailyRate = item.DailyRate,
            categoryId = item.CategoryId,
            latitude = item.Latitude,
            longitude = item.Longitude
        });

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"CreateItem failed: {response.StatusCode} - {errorContent}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Item>();
    }

    public async Task<Item?> UpdateItemAsync(int id, Item item)
    {
        EnsureAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"/items/{id}", new
        {
            title = item.Title,
            description = item.Description,
            dailyRate = item.DailyRate,
            isAvailable = item.IsAvailable
        });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Item>();
    }

    private class ItemsResponse
    {
        public List<Item> Items { get; set; } = new();
    }
}