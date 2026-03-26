using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public class ItemRepository : IItemRepository
{
    // Depends on AppDbContext — Entity Framework Core does the actual database work
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Item>> GetAllAsync()
    {
        return await _context.Items
            .Include(i => i.Category)   // also load the related Category
            .Include(i => i.Owner)      // also load the related Owner
            .OrderBy(i => i.Title)
            .ToListAsync();
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .FirstOrDefaultAsync(i => i.Id == id);
            // FirstOrDefaultAsync returns null if not found
            // That's why the return type is Item? (nullable)
    }

    public async Task<Item> CreateAsync(Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync(); // writes to the database
        return item; // now has its Id set by the database
    }

    public async Task UpdateAsync(Item item)
    {
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }
}