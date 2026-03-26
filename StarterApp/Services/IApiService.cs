using StarterApp.Database.Models;

namespace StarterApp.Services;

public interface IApiService
{

    // Items
    Task<List<Item>> GetItemsAsync();
    Task<Item?> GetItemAsync(int id);
    Task<Item?> CreateItemAsync(Item item);
    Task<Item?> UpdateItemAsync(int id, Item item);
}