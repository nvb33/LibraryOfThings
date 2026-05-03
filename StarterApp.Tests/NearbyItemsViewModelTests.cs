using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class NearbyItemsViewModelTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<ILocationService> _mockLocationService;
    private readonly NearbyItemsViewModel _viewModel;

    public NearbyItemsViewModelTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _mockLocationService = new Mock<ILocationService>();
        _viewModel = new NearbyItemsViewModel(
            _mockItemRepository.Object,
            _mockLocationService.Object);
    }

    [Fact]
    public void Items_DefaultsToEmpty()
    {
        Assert.Empty(_viewModel.Items);
    }

    [Fact]
    public void RadiusKm_DefaultsToFive()
    {
        Assert.Equal(5.0, _viewModel.RadiusKm);
    }

    [Fact]
    public void HasResults_DefaultsToFalse()
    {
        Assert.False(_viewModel.HasResults);
    }

    [Fact]
    public void SearchQuery_DefaultsToEmpty()
    {
        Assert.Equal(string.Empty, _viewModel.SearchQuery);
    }

    [Fact]
    public async Task SearchNearby_WhenLocationNull_SetsErrorMessage()
    {
        // Arrange
        _mockLocationService
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((ValueTuple<double, double>?)null);

        // Act
        await _viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
    }

    [Fact]
    public async Task SearchNearby_WhenLocationAvailable_CallsRepository()
    {
        // Arrange
        _mockLocationService
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((55.9533, -3.1883));
        _mockItemRepository
            .Setup(r => r.GetNearbyAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new List<Item>());

        // Act
        await _viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        _mockItemRepository.Verify(r => r.GetNearbyAsync(
            55.9533, -3.1883, 5.0), Times.Once);
    }

    [Fact]
    public async Task SearchNearby_PopulatesItemsCollection()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", CategoryName = "Tools" },
            new Item { Id = 2, Title = "Tent", CategoryName = "Sports" }
        };
        _mockLocationService
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((55.9533, -3.1883));
        _mockItemRepository
            .Setup(r => r.GetNearbyAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(items);

        // Act
        await _viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, _viewModel.Items.Count);
    }

    [Fact]
    public async Task SearchNearby_PopulatesCategoryFilters()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", CategoryName = "Tools" },
            new Item { Id = 2, Title = "Tent", CategoryName = "Sports" }
        };
        _mockLocationService
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((55.9533, -3.1883));
        _mockItemRepository
            .Setup(r => r.GetNearbyAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(items);

        // Act
        await _viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert — "All categories" + Tools + Sports
        Assert.Equal(3, _viewModel.CategoryFilters.Count);
    }

    [Fact]
    public async Task SearchNearby_SetsHasResultsTrue_WhenItemsFound()
    {
        // Arrange
        _mockLocationService
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((55.9533, -3.1883));
        _mockItemRepository
            .Setup(r => r.GetNearbyAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new List<Item> { new Item { Id = 1, Title = "Drill" } });

        // Act
        await _viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.HasResults);
    }

    [Fact]
    public async Task CategoryFilter_FiltersItemsByCategory()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", CategoryName = "Tools" },
            new Item { Id = 2, Title = "Tent", CategoryName = "Sports" }
        };
        _mockLocationService
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((55.9533, -3.1883));
        _mockItemRepository
            .Setup(r => r.GetNearbyAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(items);
        await _viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Act
        _viewModel.SelectedCategoryFilter = "Tools";

        // Assert
        Assert.Single(_viewModel.Items);
        Assert.Equal("Drill", _viewModel.Items[0].Title);
    }

    [Fact]
    public async Task SearchNearby_SetsIsBusyFalseAfterCompletion()
    {
        // Arrange
        _mockLocationService
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((55.9533, -3.1883));
        _mockItemRepository
            .Setup(r => r.GetNearbyAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(new List<Item>());

        // Act
        await _viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsBusy);
    }
}