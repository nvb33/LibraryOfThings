using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Defines the contract for all communication with the remote REST API.
/// Provides methods for items, categories, rentals and reviews.
/// </summary>
public interface IApiService
{
    // Items

    /// <summary>
    /// Retrieves all available items from the API.
    /// </summary>
    /// <returns>A list of all items, or an empty list if none found.</returns>
    Task<List<Item>> GetItemsAsync();

    /// <summary>
    /// Retrieves a single item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>The matching item, or null if not found.</returns>
    Task<Item?> GetItemAsync(int id);

    /// <summary>
    /// Creates a new item listing on the API.
    /// </summary>
    /// <param name="item">The item to create, populated with title, description, rate and location.</param>
    /// <returns>The created item with its assigned ID, or null if creation failed.</returns>
    Task<Item?> CreateItemAsync(Item item);

    /// <summary>
    /// Updates an existing item listing on the API.
    /// </summary>
    /// <param name="id">The unique identifier of the item to update.</param>
    /// <param name="item">The updated item data.</param>
    /// <returns>The updated item, or null if the update failed.</returns>
    Task<Item?> UpdateItemAsync(int id, Item item);

    /// <summary>
    /// Retrieves all available item categories from the API.
    /// </summary>
    /// <returns>A list of categories, or an empty list if none found.</returns>
    Task<List<Category>> GetCategoriesAsync();

    /// <summary>
    /// Retrieves items within a given radius of a geographic location.
    /// </summary>
    /// <param name="lat">Latitude of the search centre point.</param>
    /// <param name="lon">Longitude of the search centre point.</param>
    /// <param name="radiusKm">Search radius in kilometres. Defaults to 5km.</param>
    /// <returns>A list of nearby items sorted by distance, or an empty list if none found.</returns>
    Task<List<Item>> GetNearbyItemsAsync(double lat, double lon, double radiusKm = 5);

    // Rentals

    /// <summary>
    /// Creates a new rental request for an item.
    /// The authenticated user becomes the borrower.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item to rent.</param>
    /// <param name="startDate">The desired start date in yyyy-MM-dd format.</param>
    /// <param name="endDate">The desired end date in yyyy-MM-dd format.</param>
    /// <returns>The created rental with status Requested, or null if creation failed.</returns>
    Task<Rental?> CreateRentalAsync(int itemId, string startDate, string endDate);

    /// <summary>
    /// Retrieves all rentals where the authenticated user is the borrower.
    /// </summary>
    /// <returns>A list of outgoing rentals, or an empty list if none found.</returns>
    Task<List<Rental>> GetOutgoingRentalsAsync();

    /// <summary>
    /// Retrieves all rental requests made for items owned by the authenticated user.
    /// </summary>
    /// <returns>A list of incoming rental requests, or an empty list if none found.</returns>
    Task<List<Rental>> GetIncomingRentalsAsync();

    /// <summary>
    /// Updates the status of an existing rental.
    /// Valid transitions depend on the current status and the authenticated user's role.
    /// </summary>
    /// <param name="rentalId">The unique identifier of the rental to update.</param>
    /// <param name="status">The new status value. Valid values: Approved, Rejected,
    /// Out for Rent, Returned, Completed.</param>
    /// <returns>True if the update succeeded, false otherwise.</returns>
    Task<bool> UpdateRentalStatusAsync(int rentalId, string status);

    // Reviews

    /// <summary>
    /// Submits a review for a completed rental.
    /// The authenticated user must be the borrower and the rental must be Completed.
    /// </summary>
    /// <param name="rentalId">The unique identifier of the completed rental to review.</param>
    /// <param name="rating">The rating value, must be between 1 and 5 inclusive.</param>
    /// <param name="comment">An optional comment, maximum 500 characters.</param>
    /// <returns>The created review, or null if submission failed.</returns>
    Task<Review?> SubmitReviewAsync(int rentalId, int rating, string comment);

    /// <summary>
    /// Retrieves all reviews for a specific item along with summary statistics.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <returns>An ItemReviewsResult containing the reviews list, average rating and total count.</returns>
    Task<ItemReviewsResult> GetItemReviewsAsync(int itemId);

    /// <summary>
    /// Retrieves the average rating for a specific item.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <returns>The average rating as a double, or 0.0 if no reviews exist.</returns>
    Task<double> GetItemAverageRatingAsync(int itemId);
}

/// <summary>
/// Represents the result of a request to retrieve reviews for an item,
/// including the reviews themselves and summary statistics.
/// </summary>
public class ItemReviewsResult
{
    /// <summary>Gets or sets the list of reviews for the item.</summary>
    public List<Review> Reviews { get; set; } = new();

    /// <summary>Gets or sets the average rating across all reviews, or null if no reviews exist.</summary>
    public double? AverageRating { get; set; }

    /// <summary>Gets or sets the total number of reviews for the item.</summary>
    public int TotalReviews { get; set; }
}