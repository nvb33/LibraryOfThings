using System.Text.Json.Serialization;

namespace StarterApp.Database.Models;

/// <summary>
/// Represents a rental agreement between a borrower and an item owner.
/// Maps directly to the rental object returned by the REST API.
/// </summary>
public class Rental
{
    /// <summary>Gets or sets the unique identifier of the rental.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Gets or sets the unique identifier of the item being rented.</summary>
    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }

    /// <summary>Gets or sets the title of the item being rented.</summary>
    [JsonPropertyName("itemTitle")]
    public string ItemTitle { get; set; } = string.Empty;

    /// <summary>Gets or sets the unique identifier of the borrower.</summary>
    [JsonPropertyName("borrowerId")]
    public int BorrowerId { get; set; }

    /// <summary>Gets or sets the full name of the borrower.</summary>
    [JsonPropertyName("borrowerName")]
    public string BorrowerName { get; set; } = string.Empty;

    /// <summary>Gets or sets the unique identifier of the item owner.</summary>
    [JsonPropertyName("ownerId")]
    public int OwnerId { get; set; }

    /// <summary>Gets or sets the full name of the item owner.</summary>
    [JsonPropertyName("ownerName")]
    public string OwnerName { get; set; } = string.Empty;

    /// <summary>Gets or sets the rental start date in yyyy-MM-dd format.</summary>
    [JsonPropertyName("startDate")]
    public string StartDate { get; set; } = string.Empty;

    /// <summary>Gets or sets the rental end date in yyyy-MM-dd format.</summary>
    [JsonPropertyName("endDate")]
    public string EndDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the rental.
    /// Valid values: Requested, Approved, Rejected, Out for Rent, Overdue, Returned, Completed.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the total price of the rental in GBP.</summary>
    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    /// <summary>Gets or sets the date and time the rental was requested.</summary>
    [JsonPropertyName("requestedAt")]
    public DateTime? RequestedAt { get; set; }

    /// <summary>Gets or sets the date and time the rental was approved by the owner.</summary>
    [JsonPropertyName("approvedAt")]
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Gets the total price formatted as a GBP currency string.
    /// Example: £25.00
    /// </summary>
    public string FormattedPrice => $"£{TotalPrice:F2}";

    /// <summary>
    /// Gets a human-readable representation of the rental period.
    /// Example: 2025-06-01 → 2025-06-07
    /// </summary>
    public string FormattedDates => $"{StartDate} → {EndDate}";

    /// <summary>
    /// Gets the hex colour code corresponding to the current rental status,
    /// used to colour-code status labels in the UI.
    /// </summary>
    public string StatusColour => Status switch
    {
        "Requested"   => "#FF9800",
        "Approved"    => "#4CAF50",
        "Rejected"    => "#F44336",
        "Out for Rent"=> "#2196F3",
        "Returned"    => "#9C27B0",
        "Completed"   => "#607D8B",
        _             => "#607D8B"
    };

    /// <summary>
    /// Gets a value indicating whether this rental is overdue.
    /// A rental is overdue when its status is Out for Rent and the end date has passed.
    /// </summary>
    public bool IsOverdue =>
        Status == "Out for Rent" &&
        DateTime.TryParse(EndDate, out var end) &&
        end.Date < DateTime.Today;

    /// <summary>
    /// Gets a human-readable overdue warning message including the original end date,
    /// or an empty string if the rental is not overdue.
    /// </summary>
    public string OverdueMessage =>
        IsOverdue ? $"Overdue since {DateTime.Parse(EndDate):dd MMM yyyy}" : string.Empty;
}