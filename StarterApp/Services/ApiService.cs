using System.Net.Http.Json;
using System.Text.Json.Serialization;
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
            System.Diagnostics.Debug.WriteLine(
                $"CreateItem failed: {response.StatusCode} - {errorContent}");
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

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var response = await _httpClient
            .GetFromJsonAsync<CategoriesResponse>("/categories");
        return response?.Categories ?? new List<Category>();
    }

    private class NearbyItemsResponse
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new();
    }

    public async Task<List<Item>> GetNearbyItemsAsync(double lat, double lon, double radiusKm = 5)
    {
        EnsureAuthHeader();
        var url = $"/items/nearby?lat={lat}&lon={lon}&radius={radiusKm}";
        System.Diagnostics.Debug.WriteLine($"Calling nearby: {url}");

        try
        {
            // Get raw response first so we can log it
            var rawResponse = await _httpClient.GetAsync(url);
            var rawJson = await rawResponse.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Nearby raw response: {rawJson[..Math.Min(200, rawJson.Length)]}");

            var response = await System.Text.Json.JsonSerializer.DeserializeAsync<NearbyItemsResponse>(
                await rawResponse.Content.ReadAsStreamAsync());

            System.Diagnostics.Debug.WriteLine($"Nearby items count: {response?.Items?.Count ?? 0}");
            return response?.Items ?? new List<Item>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Nearby deserialisation error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return new List<Item>();
        }
    }

    public async Task<Rental?> CreateRentalAsync(int itemId, string startDate, string endDate)
    {
        EnsureAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("/rentals", new
        {
            itemId,
            startDate,
            endDate
        });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"CreateRental failed: {response.StatusCode} - {error}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Rental>();
    }

    public async Task<List<Rental>> GetOutgoingRentalsAsync()
    {
        EnsureAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<RentalsResponse>("/rentals/outgoing");
        return response?.Rentals ?? new List<Rental>();
    }

    public async Task<List<Rental>> GetIncomingRentalsAsync()
    {
        EnsureAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<RentalsResponse>("/rentals/incoming");
        return response?.Rentals ?? new List<Rental>();
    }

    public async Task<bool> UpdateRentalStatusAsync(int rentalId, string status)
    {
        EnsureAuthHeader();
        var response = await _httpClient.PatchAsJsonAsync($"/rentals/{rentalId}/status", new
        {
            status
        });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"UpdateRentalStatus failed: {response.StatusCode} - {error}");
            return false;
        }

        return true;
    }

    public async Task<Review?> SubmitReviewAsync(int rentalId, int rating, string comment)
    {
        EnsureAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("/reviews", new
        {
            rentalId,
            rating,
            comment
        });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"SubmitReview failed: {response.StatusCode} - {error}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Review>();
    }

    public async Task<ItemReviewsResult> GetItemReviewsAsync(int itemId)
    {
        EnsureAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<ReviewsResponse>($"/items/{itemId}/reviews");

        if (response == null)
            return new ItemReviewsResult();

        return new ItemReviewsResult
        {
            Reviews = response.Reviews,
            AverageRating = response.AverageRating,
            TotalReviews = response.TotalReviews
        };
    }

    public async Task<double> GetItemAverageRatingAsync(int itemId)
    {
        var response = await _httpClient.GetFromJsonAsync<ReviewsResponse>($"/items/{itemId}/reviews");
        return response?.AverageRating ?? 0.0;
    }

    private class RentalsResponse
    {
        [JsonPropertyName("rentals")]
        public List<Rental> Rentals { get; set; } = new();
    }

    // Private helper classes to deserialise API list responses
    private class ItemsResponse
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new();
    }

    private class CategoriesResponse
    {
        [JsonPropertyName("categories")]
        public List<Category> Categories { get; set; } = new();
    }

    private class ReviewsResponse
    {
        [JsonPropertyName("reviews")]
        public List<Review> Reviews { get; set; } = new();

        [JsonPropertyName("averageRating")]
        public double AverageRating { get; set; }

        [JsonPropertyName("totalReviews")]
        public int TotalReviews { get; set; }
    }
}