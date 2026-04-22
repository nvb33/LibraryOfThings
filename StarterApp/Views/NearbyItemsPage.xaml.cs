using StarterApp.ViewModels;

namespace StarterApp.Views;

/// <summary>
/// View for the Items Near Me page, allowing users to search for available
/// rental items within a configurable radius of their current location.
/// Displays results in a list with distance information and a radius slider.
/// </summary>
public partial class NearbyItemsPage : ContentPage
{
    /// <summary>
    /// Initialises a new instance of <see cref="NearbyItemsPage"/> with
    /// the provided ViewModel injected via dependency injection.
    /// </summary>
    /// <param name="viewModel">The ViewModel managing nearby search state and commands.</param>
    public NearbyItemsPage(NearbyItemsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}