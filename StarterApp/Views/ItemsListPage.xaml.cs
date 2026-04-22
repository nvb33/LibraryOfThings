using StarterApp.ViewModels;

namespace StarterApp.Views;

/// <summary>
/// View for the Items List page, displaying all available rental items
/// retrieved from the API. Reloads the list automatically each time
/// the page appears to reflect any newly created items.
/// </summary>
public partial class ItemsListPage : ContentPage
{
    /// <summary>
    /// Initialises a new instance of <see cref="ItemsListPage"/> with
    /// the provided ViewModel injected via dependency injection.
    /// </summary>
    /// <param name="viewModel">The ViewModel managing items list state and commands.</param>
    public ItemsListPage(ItemsListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    /// <summary>
    /// Called when the page becomes visible. Triggers a reload of the items
    /// list so that newly created items appear without manual refresh.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((ItemsListViewModel)BindingContext).LoadItemsCommand.Execute(null);
    }
}