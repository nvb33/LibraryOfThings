using System.Net.Http.Json;
using System.Text.Json.Serialization;
using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Authenticates users via the backend REST API and manages the JWT session token.
/// Implements <see cref="IAuthenticationService"/> for API-connected use,
/// replacing the original local BCrypt-based authentication service.
/// </summary>
public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private User? _currentUser;
    private string? _token;
    private List<string> _currentUserRoles = new();

    private const string BaseUrl = "https://set09102-api.b-davison.workers.dev";

    /// <inheritdoc/>
    public event EventHandler<bool>? AuthenticationStateChanged;

    /// <inheritdoc/>
    public bool IsAuthenticated => _currentUser != null;

    /// <inheritdoc/>
    public User? CurrentUser => _currentUser;

    /// <inheritdoc/>
    public List<string> CurrentUserRoles => _currentUserRoles;

    /// <inheritdoc/>
    public string? Token => _token;

    /// <summary>
    /// Initialises a new instance of <see cref="ApiAuthenticationService"/>
    /// and configures the HTTP client with the API base URL.
    /// </summary>
    public ApiAuthenticationService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    /// <inheritdoc/>
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

            _token = result.Token;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var userResponse = await _httpClient.GetFromJsonAsync<ApiUser>("/users/me");

            if (userResponse == null)
                return new AuthenticationResult(false, "Failed to load user profile");

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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public Task LogoutAsync()
    {
        _currentUser = null;
        _token = null;
        _currentUserRoles.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        AuthenticationStateChanged?.Invoke(this, false);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>Role-based access control is not implemented for API authentication. Always returns false.</remarks>
    public bool HasRole(string roleName) => false;

    /// <inheritdoc/>
    /// <remarks>Role-based access control is not implemented for API authentication. Always returns false.</remarks>
    public bool HasAnyRole(params string[] roleNames) => false;

    /// <inheritdoc/>
    /// <remarks>Role-based access control is not implemented for API authentication. Always returns false.</remarks>
    public bool HasAllRoles(params string[] roleNames) => false;

    /// <inheritdoc/>
    /// <remarks>Password change is not implemented for API authentication. Always returns false.</remarks>
    public Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        => Task.FromResult(false);

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