namespace StarterApp.Database.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    // Navigation — one category has many items
    public List<Item> Items { get; set; } = new();
}