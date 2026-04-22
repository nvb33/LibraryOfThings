using StarterApp.ViewModels;

namespace StarterApp.Views;

/// <summary>
/// View for the Item Detail page, displaying full information about
/// a single rental item including description, daily rate, category,
/// owner and availability. Provides navigation to request a rental
/// or view existing reviews.
/// </summary>
public partial class ItemDetailPage : ContentPage
{
    /// <summary>
    /// Initialises a new instance of <see cref="ItemDetailPage"/> with
    /// the provided ViewModel injected via dependency injection.
    /// </summary>
    /// <param name="viewModel">The ViewModel managing item detail state and commands.</param>
    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}