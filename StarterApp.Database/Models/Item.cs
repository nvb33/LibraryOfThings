namespace StarterApp.Database.Models;

public class Item
{
    // Primary key — Entity Framework Core recognises "Id" automatically
    public int Id { get; set; }

    // Basic details
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Location — used for the nearby search
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // When it was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys — relationships to other tables
    public int CategoryId { get; set; }
    public int OwnerId { get; set; }

    // Navigation properties — Entity Framework Core uses these to join tables
    // "?" means nullable
    public Category? Category { get; set; }
    public User? Owner { get; set; }

    // A computed property — not stored in Database, just convenient for the UI
    public string AvailabilityText => IsAvailable ? "Available" : "Unavailable";
    public string FormattedRate => $"£{DailyRate:F2} per day";
}