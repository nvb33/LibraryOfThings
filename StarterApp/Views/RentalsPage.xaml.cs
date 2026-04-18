using StarterApp.ViewModels;

namespace StarterApp.Views;

/// <summary>
/// View for the My Rentals page, displaying both outgoing rental requests
/// made by the current user and incoming requests for items they own.
/// Provides tab-based navigation between the two views and contextual
/// action buttons based on each rental's current status.
/// Reloads rentals automatically each time the page appears.
/// </summary>
public partial class RentalsPage : ContentPage
{
    /// <summary>
    /// Initialises a new instance of <see cref="RentalsPage"/> with
    /// the provided ViewModel injected via dependency injection.
    /// </summary>
    /// <param name="viewModel">The ViewModel managing rentals state and commands.</param>
    public RentalsPage(RentalsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    /// <summary>
    /// Called when the page becomes visible. Triggers a reload of both
    /// outgoing and incoming rentals to reflect any status changes
    /// made since the page was last visited.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((RentalsViewModel)BindingContext).LoadRentalsCommand.Execute(null);
    }
}