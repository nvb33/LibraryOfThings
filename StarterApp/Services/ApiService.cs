using System.Net.Http.Json;
using System.Text.Json.Serialization;
using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Implements <see cref="IApiService"/> by communicating with the remote REST API
/// over HTTPS using JWT bearer token authentication.
/// All methods call <see cref="EnsureAuthHeader"/> before making requests to ensure
/// the current session token is attached to every HTTP request.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthenticationService _authService;

    private const string BaseUrl = "https://set09102-api.b-davison.workers.dev";

    /// <summary>
    /// Initialises a new instance of <see cref="ApiService"/> with the provided authentication service.
    /// </summary>
    /// <param name="authService">The authentication service used to retrieve the current JWT token.</param>
    public ApiService(IAuthenticationService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    /// <summary>
    /// Ensures the HTTP client's Authorization header is set to the current JWT bearer token.
    /// Called before every API request to guarantee authentication is attached.
    /// </summary>
    private void EnsureAuthHeader()
    {
        var token = _authService.Token;
        if (token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    /// <summary>
    /// Checks an HTTP response for a 401 Unauthorized status and redirects
    /// to the login page if the token has expired or is invalid.
    /// </summary>
    /// <param name="response">The HTTP response to check.</param>
    /// <returns>True if the response was a 401 and the user has been redirected.</returns>
    private async Task<bool> HandleUnauthorizedAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            System.Diagnostics.Debug.WriteLine("401 Unauthorized — token expired, redirecting to login");
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//login");
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public async Task<List<Item>> GetItemsAsync()
    {
        EnsureAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<ItemsResponse>("/items");
        return response?.Items ?? new List<Item>();
    }

    /// <inheritdoc/>
    public async Task<Item?> GetItemAsync(int id)
    {
        EnsureAuthHeader();
        return await _httpClient.GetFromJsonAsync<Item>($"/items/{id}");
    }

    /// <inheritdoc/>
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
            if (await HandleUnauthorizedAsync(response)) return null;
            var errorContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(
                $"CreateItem failed: {response.StatusCode} - {errorContent}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Item>();
    }

    /// <inheritdoc/>
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
        {
            if (await HandleUnauthorizedAsync(response)) return null;
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Item>();
    }

    /// <inheritdoc/>
    public async Task<List<Category>> GetCategoriesAsync()
    {
        var response = await _httpClient
            .GetFromJsonAsync<CategoriesResponse>("/categories");
        return response?.Categories ?? new List<Category>();
    }

    /// <inheritdoc/>
    public async Task<List<Item>> GetNearbyItemsAsync(double lat, double lon, double radiusKm = 5)
    {
        EnsureAuthHeader();
        var url = $"/items/nearby?lat={lat}&lon={lon}&radius={radiusKm}";
        System.Diagnostics.Debug.WriteLine($"Calling nearby: {url}");

        try
        {
            var rawResponse = await _httpClient.GetAsync(url);
            var rawJson = await rawResponse.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(
                $"Nearby raw response: {rawJson[..Math.Min(200, rawJson.Length)]}");

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

    /// <inheritdoc/>
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
            if (await HandleUnauthorizedAsync(response)) return null;
            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"CreateRental failed: {response.StatusCode} - {error}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Rental>();
    }

    /// <inheritdoc/>
    public async Task<List<Rental>> GetOutgoingRentalsAsync()
    {
        EnsureAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<RentalsResponse>("/rentals/outgoing");
        return response?.Rentals ?? new List<Rental>();
    }

    /// <inheritdoc/>
    public async Task<List<Rental>> GetIncomingRentalsAsync()
    {
        EnsureAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<RentalsResponse>("/rentals/incoming");
        return response?.Rentals ?? new List<Rental>();
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateRentalStatusAsync(int rentalId, string status)
    {
        EnsureAuthHeader();
        var response = await _httpClient.PatchAsJsonAsync($"/rentals/{rentalId}/status", new
        {
            status
        });

        if (!response.IsSuccessStatusCode)
        {
            if (await HandleUnauthorizedAsync(response)) return false;
            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(
                $"UpdateRentalStatus failed: {response.StatusCode} - {error}");
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
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
            if (await HandleUnauthorizedAsync(response)) return null;
            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"SubmitReview failed: {response.StatusCode} - {error}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Review>();
    }

    /// <inheritdoc/>
    public async Task<ItemReviewsResult> GetItemReviewsAsync(int itemId)
    {
        EnsureAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<ReviewsResponse>($"/items/{itemId}/reviews");

        if (response == null)
            return new ItemReviewsResult();

        return new ItemReviewsResult
        {
            Reviews = response.Reviews,
            AverageRating = response.AverageRating ?? 0.0,
            TotalReviews = response.TotalReviews
        };
    }

    /// <inheritdoc/>
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

    private class NearbyItemsResponse
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new();
    }

    private class ReviewsResponse
    {
        [JsonPropertyName("reviews")]
        public List<Review> Reviews { get; set; } = new();

        [JsonPropertyName("averageRating")]
        public double? AverageRating { get; set; }

        [JsonPropertyName("totalReviews")]
        public int TotalReviews { get; set; }
    }
}