using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class ItemDetailViewModelTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly ItemDetailViewModel _viewModel;

    public ItemDetailViewModelTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockAuthService.Setup(a => a.CurrentUser).Returns(new User { Id = 1 });
        _viewModel = new ItemDetailViewModel(
            _mockItemRepository.Object,
            _mockAuthService.Object);
    }

    [Fact]
    public void Item_DefaultsToNull()
    {
        Assert.Null(_viewModel.Item);
    }

    [Fact]
    public void IsBusy_DefaultsToFalse()
    {
        Assert.False(_viewModel.IsBusy);
    }

    [Fact]
    public void CanRequestRental_WhenItemIsNull_ReturnsFalse()
    {
        // Arrange
        _viewModel.Item = null;

        // Assert
        Assert.False(_viewModel.CanRequestRental);
    }

    [Fact]
    public void CanRequestRental_WhenOwnerIsCurrentUser_ReturnsFalse()
    {
        // Arrange — current user id is 1, item owner id is also 1
        _viewModel.Item = new Item { Id = 1, OwnerId = 1, Title = "Drill" };

        // Assert
        Assert.False(_viewModel.CanRequestRental);
    }

    [Fact]
    public void CanRequestRental_WhenOwnerIsDifferentUser_ReturnsTrue()
    {
        // Arrange — current user id is 1, item owner id is 2
        _viewModel.Item = new Item { Id = 1, OwnerId = 2, Title = "Drill" };

        // Assert
        Assert.True(_viewModel.CanRequestRental);
    }

    [Fact]
    public async Task LoadItem_SetsItemFromRepository()
    {
        // Arrange
        var item = new Item { Id = 1, Title = "Drill", OwnerId = 2 };
        _mockItemRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        // Act
        _viewModel.ItemId = 1;
        await Task.Delay(100); // allow async load to complete

        // Assert
        Assert.NotNull(_viewModel.Item);
        Assert.Equal("Drill", _viewModel.Item.Title);
    }

    [Fact]
    public async Task LoadItem_SetsIsBusyFalseAfterCompletion()
    {
        // Arrange
        _mockItemRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Item { Id = 1, Title = "Drill" });

        // Act
        await _viewModel.LoadItemCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsBusy);
    }
}