using System.Text.Json.Serialization;

namespace StarterApp.Database.Models;

public class Rental
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }

    [JsonPropertyName("itemTitle")]
    public string ItemTitle { get; set; } = string.Empty;

    [JsonPropertyName("borrowerId")]
    public int BorrowerId { get; set; }

    [JsonPropertyName("borrowerName")]
    public string BorrowerName { get; set; } = string.Empty;

    [JsonPropertyName("ownerId")]
    public int OwnerId { get; set; }

    [JsonPropertyName("ownerName")]
    public string OwnerName { get; set; } = string.Empty;

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("endDate")]
    public string EndDate { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("requestedAt")]
    public DateTime? RequestedAt { get; set; }

    [JsonPropertyName("approvedAt")]
    public DateTime? ApprovedAt { get; set; }

    // Computed properties for the UI
    public string FormattedPrice => $"£{TotalPrice:F2}";
    public string FormattedDates => $"{StartDate} → {EndDate}";

    public string StatusColour => Status switch
    {
        "Requested" => "#FF9800",   // orange
        "Approved" => "#4CAF50",    // green
        "Rejected" => "#F44336",    // red
        "Out for Rent" => "#2196F3",// blue
        "Returned" => "#9C27B0",    // purple
        "Completed" => "#607D8B",   // grey
        _ => "#607D8B"
    };
}