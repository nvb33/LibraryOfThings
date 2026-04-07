using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class RentalsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Rental> _outgoingRentals = new();

    [ObservableProperty]
    private ObservableCollection<Rental> _incomingRentals = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Controls which tab is visible
    [ObservableProperty]
    private bool _showingOutgoing = true;

    public bool ShowingIncoming => !ShowingOutgoing;

    public RentalsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private void ShowOutgoing()
    {
        ShowingOutgoing = true;
        OnPropertyChanged(nameof(ShowingIncoming));
    }

    [RelayCommand]
    private void ShowIncoming()
    {
        ShowingOutgoing = false;
        OnPropertyChanged(nameof(ShowingIncoming));
    }

    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var outgoing = await _apiService.GetOutgoingRentalsAsync();
            OutgoingRentals = new ObservableCollection<Rental>(outgoing);

            var incoming = await _apiService.GetIncomingRentalsAsync();
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

    [RelayCommand]
    private async Task ApproveRentalAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Approved");
    }

    [RelayCommand]
    private async Task RejectRentalAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Rejected");
    }

    [RelayCommand]
    private async Task MarkOutForRentAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Out for Rent");
    }

    [RelayCommand]
    private async Task MarkReturnedAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Returned");
    }

    [RelayCommand]
    private async Task CompleteRentalAsync(Rental rental)
    {
        await UpdateStatusAsync(rental, "Completed");
    }

    private async Task UpdateStatusAsync(Rental rental, string newStatus)
    {
        IsBusy = true;
        try
        {
            var success = await _apiService.UpdateRentalStatusAsync(rental.Id, newStatus);
            if (success)
            {
                // Reload to reflect the new status
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

    [RelayCommand]
    private async Task NavigateToReviewAsync(Rental rental)
    {
        await Shell.Current.GoToAsync(
            $"submitreview?rentalId={rental.Id}&itemTitle={Uri.EscapeDataString(rental.ItemTitle)}");
    }
}