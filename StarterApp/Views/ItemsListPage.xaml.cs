using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class ItemsListPage : ContentPage
{
    public ItemsListPage(ItemsListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Reload the list every time the page appears
        // This ensures new items show up after returning from CreateItemPage
        ((ItemsListViewModel)BindingContext).LoadItemsCommand.Execute(null);
    }
}