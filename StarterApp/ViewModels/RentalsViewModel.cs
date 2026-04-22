using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the My Rentals page, managing both outgoing rental requests
/// made by the current user and incoming requests for items they own.
/// Delegates all business logic to IRentalService.
/// </summary>
public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;

    /// <summary>Gets or sets the collection of rentals where the current user is the borrower.</summary>
    [ObservableProperty]
    private ObservableCollection<Rental> _outgoingRentals = new();

    /// <summary>Gets or sets the collection of rental requests for items owned by the current user.</summary>
    [ObservableProperty]
    private ObservableCollection<Rental> _incomingRentals = new();

    /// <summary>Gets or sets a value indicating whether an operation is in progress.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets a value indicating whether no operation is currently in progress.</summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>Gets or sets the error message to display when an operation fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>Gets or sets a value indicating whether the outgoing rentals tab is active.</summary>
    [ObservableProperty]
    private bool _showingOutgoing = true;

    /// <summary>Gets a value indicating whether the incoming rentals tab is active.</summary>
    public bool ShowingIncoming => !ShowingOutgoing;

    /// <summary>
    /// Initialises a new instance of <see cref="RentalsViewModel"/>.
    /// </summary>
    /// <param name="rentalService">The service providing rental business logic.</param>
    public RentalsViewModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    /// <summary>Switches the active tab to outgoing rentals.</summary>
    [RelayCommand]
    private void ShowOutgoing()
    {
        ShowingOutgoing = true;
        OnPropertyChanged(nameof(ShowingIncoming));
    }

    /// <summary>Switches the active tab to incoming rentals.</summary>
    [RelayCommand]
    private void ShowIncoming()
    {
        ShowingOutgoing = false;
        OnPropertyChanged(nameof(ShowingIncoming));
    }

    /// <summary>
    /// Loads both outgoing and incoming rentals from the service.
    /// Skips execution if a load is already in progress.
    /// </summary>
    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var outgoing = await _rentalService.GetOutgoingRentalsAsync();
            OutgoingRentals = new ObservableCollection<Rental>(outgoing);

            var incoming = await _rentalService.GetIncomingRentalsAsync();
            IncomingRentals = new ObservableCollection<Rental>(incoming);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load rentals: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"LoadRentals error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Approves an incoming rental request via the rental service.</summary>
    [RelayCommand]
    private async Task ApproveRentalAsync(Rental rental)
    {
        await UpdateAsync(() => _rentalService.ApproveRentalAsync(rental),
            $"Cannot approve rental with status '{rental.Status}'.");
    }

    /// <summary>Rejects an incoming rental request via the rental service.</summary>
    [RelayCommand]
    private async Task RejectRentalAsync(Rental rental)
    {
        await UpdateAsync(() => _rentalService.RejectRentalAsync(rental),
            $"Cannot reject rental with status '{rental.Status}'.");
    }

    /// <summary>Marks an approved rental as Out for Rent via the rental service.</summary>
    [RelayCommand]
    private async Task MarkOutForRentAsync(Rental rental)
    {
        await UpdateAsync(() => _rentalService.MarkOutForRentAsync(rental),
            $"Cannot mark rental as Out for Rent with status '{rental.Status}'.");
    }

    /// <summary>Marks an active rental as Returned via the rental service.</summary>
    [RelayCommand]
    private async Task MarkReturnedAsync(Rental rental)
    {
        await UpdateAsync(() => _rentalService.MarkReturnedAsync(rental),
            $"Cannot mark rental as Returned with status '{rental.Status}'.");
    }

    /// <summary>Marks a returned rental as Completed via the rental service.</summary>
    [RelayCommand]
    private async Task CompleteRentalAsync(Rental rental)
    {
        await UpdateAsync(() => _rentalService.CompleteRentalAsync(rental),
            $"Cannot mark rental as Completed with status '{rental.Status}'.");
    }

    /// <summary>
    /// Executes a rental update operation, reloading rentals on success
    /// or setting an error message on failure.
    /// </summary>
    /// <param name="operation">The service operation to execute.</param>
    /// <param name="failureMessage">The message to display if the operation fails.</param>
    private async Task UpdateAsync(Func<Task<bool>> operation, string failureMessage)
    {
        IsBusy = true;
        try
        {
            var success = await operation();
            if (success)
                await LoadRentalsAsync();
            else
                ErrorMessage = failureMessage;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Navigates to the Submit Review page for a completed rental.</summary>
    [RelayCommand]
    private async Task NavigateToReviewAsync(Rental rental)
    {
        if (!_rentalService.CanReview(rental))
        {
            ErrorMessage = "Reviews can only be submitted for completed rentals.";
            return;
        }
        await Shell.Current.GoToAsync(
            $"submitreview?rentalId={rental.Id}&itemTitle={Uri.EscapeDataString(rental.ItemTitle)}");
    }
}