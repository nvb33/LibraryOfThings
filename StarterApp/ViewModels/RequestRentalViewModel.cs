using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class RequestRentalViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today.AddDays(2);

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Minimum selectable date is tomorrow
    public DateTime MinDate => DateTime.Today.AddDays(1);

    public RequestRentalViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task SubmitRentalAsync()
    {
        if (StartDate >= EndDate)
        {
            ErrorMessage = "End date must be after start date.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var rental = await _apiService.CreateRentalAsync(
                ItemId,
                StartDate.ToString("yyyy-MM-dd"),
                EndDate.ToString("yyyy-MM-dd")
            );

            if (rental != null)
            {
                // Navigate back and show confirmation
                await Shell.Current.DisplayAlert(
                    "Request Sent",
                    "Your rental request has been submitted. " +
                    "The owner will review it shortly.",
                    "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "Failed to submit rental request. " +
                               "The item may not be available for these dates.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}