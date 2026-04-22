using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Implements rental business logic, validating state transitions and
/// business rules before delegating data operations to IRentalRepository.
/// Keeps business logic out of ViewModels and repositories.
/// </summary>
public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="RentalService"/>.
    /// </summary>
    /// <param name="rentalRepository">The repository used for rental data access.</param>
    public RentalService(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Rental>> GetOutgoingRentalsAsync()
    {
        return await _rentalRepository.GetOutgoingAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Rental>> GetIncomingRentalsAsync()
    {
        return await _rentalRepository.GetIncomingAsync();
    }

    /// <inheritdoc/>
    public async Task<Rental?> CreateRentalAsync(int itemId, DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("End date must be after start date.");

        if (startDate < DateTime.Today)
            throw new ArgumentException("Start date cannot be in the past.");

        var rental = new Rental
        {
            ItemId = itemId,
            StartDate = startDate.ToString("yyyy-MM-dd"),
            EndDate = endDate.ToString("yyyy-MM-dd")
        };

        return await _rentalRepository.AddAsync(rental);
    }

    /// <inheritdoc/>
    public async Task<bool> ApproveRentalAsync(Rental rental)
    {
        if (!CanApprove(rental))
            return false;

        return await _rentalRepository.UpdateStatusAsync(rental.Id, "Approved");
    }

    /// <inheritdoc/>
    public async Task<bool> RejectRentalAsync(Rental rental)
    {
        if (!CanReject(rental))
            return false;

        return await _rentalRepository.UpdateStatusAsync(rental.Id, "Rejected");
    }

    /// <inheritdoc/>
    public async Task<bool> MarkOutForRentAsync(Rental rental)
    {
        if (!CanMarkOutForRent(rental))
            return false;

        return await _rentalRepository.UpdateStatusAsync(rental.Id, "Out for Rent");
    }

    /// <inheritdoc/>
    public async Task<bool> MarkReturnedAsync(Rental rental)
    {
        if (!CanMarkReturned(rental))
            return false;

        return await _rentalRepository.UpdateStatusAsync(rental.Id, "Returned");
    }

    /// <inheritdoc/>
    public async Task<bool> CompleteRentalAsync(Rental rental)
    {
        if (!CanComplete(rental))
            return false;

        return await _rentalRepository.UpdateStatusAsync(rental.Id, "Completed");
    }

    /// <inheritdoc/>
    public bool CanApprove(Rental rental) =>
        rental.Status == "Requested";

    /// <inheritdoc/>
    public bool CanReject(Rental rental) =>
        rental.Status == "Requested";

    /// <inheritdoc/>
    public bool CanMarkOutForRent(Rental rental) =>
        rental.Status == "Approved";

    /// <inheritdoc/>
    public bool CanMarkReturned(Rental rental) =>
        rental.Status == "Out for Rent" || rental.Status == "Overdue";

    /// <inheritdoc/>
    public bool CanComplete(Rental rental) =>
        rental.Status == "Returned";

    /// <inheritdoc/>
    public bool CanReview(Rental rental) =>
        rental.Status == "Completed";

    /// <inheritdoc/>
    public decimal CalculateTotalPrice(decimal dailyRate, DateTime startDate, DateTime endDate)
    {
        var days = (endDate - startDate).Days;
        return dailyRate * days;
    }
}