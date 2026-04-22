using StarterApp.ViewModels;

namespace StarterApp.Views;

/// <summary>
/// View for the Create Item page, providing a form for authenticated users
/// to list a new item for rent. Loads available categories from the API
/// automatically when the page appears.
/// </summary>
public partial class CreateItemPage : ContentPage
{
    /// <summary>
    /// Initialises a new instance of <see cref="CreateItemPage"/> with
    /// the provided ViewModel injected via dependency injection.
    /// </summary>
    /// <param name="viewModel">The ViewModel managing item creation state and commands.</param>
    public CreateItemPage(CreateItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    /// <summary>
    /// Called when the page becomes visible. Triggers loading of available
    /// categories from the API to populate the category picker.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((CreateItemViewModel)BindingContext).LoadCategoriesCommand.Execute(null);
    }
}