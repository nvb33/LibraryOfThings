using StarterApp.Database.Models;

namespace StarterApp.Services;

public interface IApiService
{
    Task<List<Item>> GetItemsAsync();
    Task<Item?> GetItemAsync(int id);
    Task<Item?> CreateItemAsync(Item item);
    Task<Item?> UpdateItemAsync(int id, Item item);
    Task<List<Category>> GetCategoriesAsync();
    Task<List<Item>> GetNearbyItemsAsync(double lat, double lon, double radiusKm = 5);

    // Rentals
    Task<Rental?> CreateRentalAsync(int itemId, string startDate, string endDate);
    Task<List<Rental>> GetOutgoingRentalsAsync();
    Task<List<Rental>> GetIncomingRentalsAsync();
    Task<bool> UpdateRentalStatusAsync(int rentalId, string status);

    // Reviews
    Task<Review?> SubmitReviewAsync(int rentalId, int rating, string comment);
    Task<ItemReviewsResult> GetItemReviewsAsync(int itemId);
    Task<double> GetItemAverageRatingAsync(int itemId);
}

public class ItemReviewsResult
{
    public List<Review> Reviews { get; set; } = new();
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}