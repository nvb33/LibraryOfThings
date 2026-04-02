using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class RequestRentalPage : ContentPage
{
    public RequestRentalPage(RequestRentalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}