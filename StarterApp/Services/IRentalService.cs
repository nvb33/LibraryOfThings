using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Defines the business logic contract for rental operations.
/// Validates state transitions and business rules before delegating
/// to the repository layer, keeping business logic out of ViewModels.
/// </summary>
public interface IRentalService
{
    /// <summary>
    /// Retrieves all rentals where the authenticated user is the borrower.
    /// </summary>
    /// <returns>A collection of outgoing rental requests.</returns>
    Task<IEnumerable<Rental>> GetOutgoingRentalsAsync();

    /// <summary>
    /// Retrieves all rental requests for items owned by the authenticated user.
    /// </summary>
    /// <returns>A collection of incoming rental requests.</returns>
    Task<IEnumerable<Rental>> GetIncomingRentalsAsync();

    /// <summary>
    /// Creates a new rental request after validating the selected dates.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item to rent.</param>
    /// <param name="startDate">The desired start date of the rental.</param>
    /// <param name="endDate">The desired end date of the rental.</param>
    /// <returns>The created rental, or null if creation failed.</returns>
    Task<Rental?> CreateRentalAsync(int itemId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Approves an incoming rental request. Validates the rental is in Requested status.
    /// </summary>
    /// <param name="rental">The rental to approve.</param>
    /// <returns>True if approval succeeded, false otherwise.</returns>
    Task<bool> ApproveRentalAsync(Rental rental);

    /// <summary>
    /// Rejects an incoming rental request. Validates the rental is in Requested status.
    /// </summary>
    /// <param name="rental">The rental to reject.</param>
    /// <returns>True if rejection succeeded, false otherwise.</returns>
    Task<bool> RejectRentalAsync(Rental rental);

    /// <summary>
    /// Marks an approved rental as Out for Rent. Validates the rental is in Approved status.
    /// </summary>
    /// <param name="rental">The rental to mark as out for rent.</param>
    /// <returns>True if the update succeeded, false otherwise.</returns>
    Task<bool> MarkOutForRentAsync(Rental rental);

    /// <summary>
    /// Marks an active rental as Returned. Validates the rental is Out for Rent or Overdue.
    /// </summary>
    /// <param name="rental">The rental to mark as returned.</param>
    /// <returns>True if the update succeeded, false otherwise.</returns>
    Task<bool> MarkReturnedAsync(Rental rental);

    /// <summary>
    /// Marks a returned rental as Completed. Validates the rental is in Returned status.
    /// </summary>
    /// <param name="rental">The rental to mark as completed.</param>
    /// <returns>True if the update succeeded, false otherwise.</returns>
    Task<bool> CompleteRentalAsync(Rental rental);

    /// <summary>
    /// Determines whether an approve action is valid for the given rental.
    /// </summary>
    /// <param name="rental">The rental to check.</param>
    /// <returns>True if the rental can be approved.</returns>
    bool CanApprove(Rental rental);

    /// <summary>
    /// Determines whether a reject action is valid for the given rental.
    /// </summary>
    /// <param name="rental">The rental to check.</param>
    /// <returns>True if the rental can be rejected.</returns>
    bool CanReject(Rental rental);

    /// <summary>
    /// Determines whether a mark as Out for Rent action is valid for the given rental.
    /// </summary>
    /// <param name="rental">The rental to check.</param>
    /// <returns>True if the rental can be marked as out for rent.</returns>
    bool CanMarkOutForRent(Rental rental);

    /// <summary>
    /// Determines whether a mark as Returned action is valid for the given rental.
    /// </summary>
    /// <param name="rental">The rental to check.</param>
    /// <returns>True if the rental can be marked as returned.</returns>
    bool CanMarkReturned(Rental rental);

    /// <summary>
    /// Determines whether a complete action is valid for the given rental.
    /// </summary>
    /// <param name="rental">The rental to check.</param>
    /// <returns>True if the rental can be marked as completed.</returns>
    bool CanComplete(Rental rental);

    /// <summary>
    /// Determines whether a review can be submitted for the given rental.
    /// </summary>
    /// <param name="rental">The rental to check.</param>
    /// <returns>True if the rental can be reviewed.</returns>
    bool CanReview(Rental rental);

    /// <summary>
    /// Calculates the total rental price based on daily rate and duration.
    /// </summary>
    /// <param name="dailyRate">The daily rental rate in GBP.</param>
    /// <param name="startDate">The rental start date.</param>
    /// <param name="endDate">The rental end date.</param>
    /// <returns>The total price for the rental period.</returns>
    decimal CalculateTotalPrice(decimal dailyRate, DateTime startDate, DateTime endDate);
}