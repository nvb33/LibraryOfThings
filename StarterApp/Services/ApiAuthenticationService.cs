// StarterApp/Services/ApiAuthenticationService.cs

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using StarterApp.Database.Models;

namespace StarterApp.Services;

/// Authenticates users via the backend API and stores the JWT token.
/// This replaces the local BCrypt-based AuthenticationService for API-connected use.
public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private User? _currentUser;
    private string? _token;
    private List<string> _currentUserRoles = new();

    private const string BaseUrl = "https://set09102-api.b-davison.workers.dev";

    public event EventHandler<bool>? AuthenticationStateChanged;

    public bool IsAuthenticated => _currentUser != null;
    public User? CurrentUser => _currentUser;
    public List<string> CurrentUserRoles => _currentUserRoles;
    public string? Token => _token;

    public ApiAuthenticationService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/token", new
            {
                email,
                password
            });

            if (!response.IsSuccessStatusCode)
                return new AuthenticationResult(false, "Invalid email or password");

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            if (result?.Token == null)
                return new AuthenticationResult(false, "No token received from server");

            // Store the token
            _token = result.Token;

            // Fetch the user profile using the token
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var userResponse = await _httpClient.GetFromJsonAsync<ApiUser>("/users/me");

            if (userResponse == null)
                return new AuthenticationResult(false, "Failed to load user profile");

            // Map API user to our local User model
            _currentUser = new User
            {
                Id = userResponse.Id,
                FirstName = userResponse.FirstName,
                LastName = userResponse.LastName,
                Email = userResponse.Email
            };

            AuthenticationStateChanged?.Invoke(this, true);
            return new AuthenticationResult(true, "Login successful");
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, $"Login failed: {ex.Message}");
        }
    }

    public async Task<AuthenticationResult> RegisterAsync(
        string firstName, string lastName, string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/register", new
            {
                firstName,
                lastName,
                email,
                password
            });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiError>();
                return new AuthenticationResult(false, error?.Message ?? "Registration failed");
            }

            return new AuthenticationResult(true, "Registration successful. Please log in.");
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, $"Registration failed: {ex.Message}");
        }
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        _token = null;
        _currentUserRoles.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        AuthenticationStateChanged?.Invoke(this, false);
        return Task.CompletedTask;
    }

    // The API doesn't have roles in the same way — default to false for admin checks
    public bool HasRole(string roleName) => false;
    public bool HasAnyRole(params string[] roleNames) => false;
    public bool HasAllRoles(params string[] roleNames) => false;

    public Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        => Task.FromResult(false); // Not implemented for API auth

    // Private classes to deserialise API responses
    private class TokenResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }
    }

    private class ApiUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }

    private class ApiError
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}