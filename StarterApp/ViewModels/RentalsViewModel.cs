using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the My Rentals page, managing both outgoing rental requests
/// made by the current user and incoming requests for items they own.
/// </summary>
public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalRepository _rentalRepository;

    /// <summary>Gets or sets the collection of rentals where the current user is the borrower.</summary>
    [ObservableProperty]
    private ObservableCollection<Rental> _outgoingRentals = new();

    /// <summary>Gets or sets the collection of rental requests for items owned by the current user.</summary>
    [ObservableProperty]
    private ObservableCollection<Rental> _incomingRentals = new();

    /// <summary>Gets or sets a value indicating whether an API operation is in progress.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets a value indicating whether no API operation is currently in progress.</summary>
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
    /// <param name="rentalRepository">The repository used to retrieve and update rental data.</param>
    public RentalsViewModel(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
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
    /// Loads both outgoing and incoming rentals from the repository.
    /// </summary>
    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var outgoing = await _rentalRepository.GetOutgoingAsync();
            OutgoingRentals = new ObservableCollection<Rental>(outgoing);

            var incoming = await _rentalRepository.GetIncomingAsync();
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

    /// <summary>Approves an incoming rental request.</summary>
    [RelayCommand]
    private async Task ApproveRentalAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Approved");
    }

    /// <summary>Rejects an incoming rental request.</summary>
    [RelayCommand]
    private async Task RejectRentalAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Rejected");
    }

    /// <summary>Marks an approved rental as Out for Rent.</summary>
    [RelayCommand]
    private async Task MarkOutForRentAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Out for Rent");
    }

    /// <summary>Marks an active rental as Returned.</summary>
    [RelayCommand]
    private async Task MarkReturnedAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Returned");
    }

    /// <summary>Marks a returned rental as Completed.</summary>
    [RelayCommand]
    private async Task CompleteRentalAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Completed");
    }

    /// <summary>
    /// Sends a status update to the repository and reloads rentals on success.
    /// </summary>
    private async Task UpdateStatusAsync(Rental rental, string newStatus)
    {
        IsBusy = true;
        try
        {
            var success = await _rentalRepository.UpdateStatusAsync(rental.Id, newStatus);
            if (success)
            {
                await LoadRentalsAsync();
            }
            else
            {
                ErrorMessage = $"Failed to update rental status to {newStatus}.";
            }
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
        await Shell.Current.GoToAsync(
            $"submitreview?rentalId={rental.Id}&itemTitle={Uri.EscapeDataString(rental.ItemTitle)}");
    }
}