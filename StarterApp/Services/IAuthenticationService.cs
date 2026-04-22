using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Defines the contract for user authentication and session management.
/// Provides methods for login, registration, logout and role-based access control.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Raised when the authentication state changes, e.g. on login or logout.
    /// The boolean parameter indicates whether the user is now authenticated.
    /// </summary>
    event EventHandler<bool>? AuthenticationStateChanged;

    /// <summary>Gets a value indicating whether a user is currently authenticated.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Gets the currently authenticated user, or null if not authenticated.</summary>
    User? CurrentUser { get; }

    /// <summary>Gets the list of roles assigned to the currently authenticated user.</summary>
    List<string> CurrentUserRoles { get; }

    /// <summary>Gets the JWT token for the current session, or null if not authenticated.</summary>
    string? Token { get; }

    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>An <see cref="AuthenticationResult"/> indicating success or failure.</returns>
    Task<AuthenticationResult> LoginAsync(string email, string password);

    /// <summary>
    /// Registers a new user account with the provided details.
    /// </summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's chosen password.</param>
    /// <returns>An <see cref="AuthenticationResult"/> indicating success or failure.</returns>
    Task<AuthenticationResult> RegisterAsync(string firstName, string lastName,
        string email, string password);

    /// <summary>
    /// Logs out the current user and clears all session data including the JWT token.
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    /// Checks whether the current user has a specific role.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns>True if the user has the role, false otherwise.</returns>
    bool HasRole(string roleName);

    /// <summary>
    /// Checks whether the current user has at least one of the specified roles.
    /// </summary>
    /// <param name="roleNames">The role names to check against.</param>
    /// <returns>True if the user has any of the specified roles, false otherwise.</returns>
    bool HasAnyRole(params string[] roleNames);

    /// <summary>
    /// Checks whether the current user has all of the specified roles.
    /// </summary>
    /// <param name="roleNames">The role names to check against.</param>
    /// <returns>True if the user has all of the specified roles, false otherwise.</returns>
    bool HasAllRoles(params string[] roleNames);

    /// <summary>
    /// Changes the authenticated user's password.
    /// </summary>
    /// <param name="currentPassword">The user's current password for verification.</param>
    /// <param name="newPassword">The new password to set.</param>
    /// <returns>True if the password was changed successfully, false otherwise.</returns>
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
}