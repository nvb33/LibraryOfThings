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
}