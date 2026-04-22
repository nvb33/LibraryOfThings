using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Request Rental page, allowing a borrower to select
/// rental dates and submit a rental request via the rental service.
/// </summary>
[QueryProperty(nameof(ItemId), "itemId")]
public partial class RequestRentalViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;

    /// <summary>Gets or sets the unique identifier of the item to be rented.</summary>
    [ObservableProperty]
    private int _itemId;

    /// <summary>Gets or sets the desired start date. Defaults to tomorrow.</summary>
    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(1);

    /// <summary>Gets or sets the desired end date. Defaults to two days from now.</summary>
    [ObservableProperty]
    private DateTime _endDate = DateTime.Today.AddDays(2);

    /// <summary>Gets or sets a value indicating whether an operation is in progress.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets or sets the error message to display when validation or submission fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>Gets the minimum selectable date, which is tomorrow.</summary>
    public DateTime MinDate => DateTime.Today.AddDays(1);

    /// <summary>
    /// Initialises a new instance of <see cref="RequestRentalViewModel"/>.
    /// </summary>
    /// <param name="rentalService">The service providing rental business logic.</param>
    public RequestRentalViewModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    /// <summary>
    /// Validates the selected dates and submits a rental request via the service.
    /// Business rule validation is performed in RentalService before the API call.
    /// </summary>
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
            var rental = await _rentalService.CreateRentalAsync(
                ItemId, StartDate, EndDate);

            if (rental != null)
            {
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
        catch (ArgumentException ex)
        {
            ErrorMessage = ex.Message;
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

    /// <summary>Navigates back without submitting a request.</summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}