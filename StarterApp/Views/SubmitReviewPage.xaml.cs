using StarterApp.ViewModels;

namespace StarterApp.Views;

/// <summary>
/// View for the Submit Review page, allowing a borrower to rate a completed
/// rental using a 1 to 5 star interactive rating with colour feedback,
/// and optionally add a written comment of up to 500 characters.
/// </summary>
public partial class SubmitReviewPage : ContentPage
{
    /// <summary>
    /// Initialises a new instance of <see cref="SubmitReviewPage"/> with
    /// the provided ViewModel injected via dependency injection.
    /// </summary>
    /// <param name="viewModel">The ViewModel managing review submission state and commands.</param>
    public SubmitReviewPage(SubmitReviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}