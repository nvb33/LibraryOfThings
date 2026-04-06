using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class ItemReviewsPage : ContentPage
{
    public ItemReviewsPage(ItemReviewsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}