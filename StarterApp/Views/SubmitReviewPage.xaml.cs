using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class SubmitReviewPage : ContentPage
{
    public SubmitReviewPage(SubmitReviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}