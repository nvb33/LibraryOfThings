using System.Net.Http.Json;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private string? _token;

    // The base URL for all requests
    private const string BaseUrl = "https://set09102-api.b-davison.workers.dev";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    // Call this after login to include the token in all future requests
    private void SetAuthToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/token", new
        {
            email,
            password
        });

        if (!response.IsSuccessStatusCode)
            return null;

        // Deserialise the JSON response into an anonymous object
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result?.Token != null)
            SetAuthToken(result.Token);

        return result?.Token;
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<ItemsResponse>("/items");
        return response?.Items ?? new List<Item>();
    }

    public async Task<Item?> GetItemAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Item>($"/items/{id}");
    }

    public async Task<Item?> CreateItemAsync(Item item)
    {
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
            return null;

        return await response.Content.ReadFromJsonAsync<Item>();
    }

    public async Task<Item?> UpdateItemAsync(int id, Item item)
    {
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

    // Private helper classes to deserialise API responses
    private class LoginResponse
    {
        public string? Token { get; set; }
        public int UserId { get; set; }
    }

    private class ItemsResponse
    {
        public List<Item> Items { get; set; } = new();
    }
}