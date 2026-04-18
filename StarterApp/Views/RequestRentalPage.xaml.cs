using StarterApp.ViewModels;

namespace StarterApp.Views;

/// <summary>
/// View for the Request Rental page, providing date pickers for the borrower
/// to select a rental period and submit a request for a specific item.
/// Validates that the end date is after the start date before submission.
/// </summary>
public partial class RequestRentalPage : ContentPage
{
    /// <summary>
    /// Initialises a new instance of <see cref="RequestRentalPage"/> with
    /// the provided ViewModel injected via dependency injection.
    /// </summary>
    /// <param name="viewModel">The ViewModel managing rental request state and commands.</param>
    public RequestRentalPage(RequestRentalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}