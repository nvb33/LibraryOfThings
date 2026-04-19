using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

/// <summary>
/// Defines the repository contract for rental data access operations,
/// extending the generic repository with rental-specific queries
/// and status management.
/// </summary>
public interface IRentalRepository : IRepository<Rental>
{
    /// <summary>
    /// Retrieves all rentals where the authenticated user is the borrower.
    /// </summary>
    /// <returns>A collection of outgoing rental requests.</returns>
    Task<IEnumerable<Rental>> GetOutgoingAsync();

    /// <summary>
    /// Retrieves all rental requests for items owned by the authenticated user.
    /// </summary>
    /// <returns>A collection of incoming rental requests.</returns>
    Task<IEnumerable<Rental>> GetIncomingAsync();

    /// <summary>
    /// Updates the status of a rental to a new value.
    /// Valid transitions are enforced server-side by the API.
    /// </summary>
    /// <param name="rentalId">The unique identifier of the rental to update.</param>
    /// <param name="status">The new status value.</param>
    /// <returns>True if the update succeeded, false otherwise.</returns>
    Task<bool> UpdateStatusAsync(int rentalId, string status);
}