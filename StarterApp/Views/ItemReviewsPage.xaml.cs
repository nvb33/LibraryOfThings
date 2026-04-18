using StarterApp.ViewModels;

namespace StarterApp.Views;

/// <summary>
/// View for the Item Reviews page, displaying all reviews submitted for
/// a specific item along with the average rating summary banner.
/// Shows reviewer name, star rating, comment and submission date for each review.
/// </summary>
public partial class ItemReviewsPage : ContentPage
{
    /// <summary>
    /// Initialises a new instance of <see cref="ItemReviewsPage"/> with
    /// the provided ViewModel injected via dependency injection.
    /// </summary>
    /// <param name="viewModel">The ViewModel managing item reviews state and commands.</param>
    public ItemReviewsPage(ItemReviewsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}