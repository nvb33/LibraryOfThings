using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class ItemsListViewModelTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly ItemsListViewModel _viewModel;

    public ItemsListViewModelTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _viewModel = new ItemsListViewModel(_mockItemRepository.Object);
    }

    [Fact]
    public void Items_DefaultsToEmpty()
    {
        Assert.Empty(_viewModel.Items);
    }

    [Fact]
    public void SearchQuery_DefaultsToEmpty()
    {
        Assert.Equal(string.Empty, _viewModel.SearchQuery);
    }

    [Fact]
    public void SelectedSortOption_DefaultsToDefault()
    {
        Assert.Equal("Default", _viewModel.SelectedSortOption);
    }

    [Fact]
    public void SelectedOwnerFilter_DefaultsToAllOwners()
    {
        Assert.Equal("All owners", _viewModel.SelectedOwnerFilter);
    }

    [Fact]
    public async Task LoadItems_PopulatesItemsCollection()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", OwnerName = "Alice", DailyRate = 10 },
            new Item { Id = 2, Title = "Ladder", OwnerName = "Bob", DailyRate = 15 }
        };
        _mockItemRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        // Act
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, _viewModel.Items.Count);
    }

    [Fact]
    public async Task LoadItems_PopulatesOwnerFilters()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", OwnerName = "Alice" },
            new Item { Id = 2, Title = "Ladder", OwnerName = "Bob" }
        };
        _mockItemRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        // Act
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert — "All owners" + Alice + Bob
        Assert.Equal(3, _viewModel.OwnerFilters.Count);
    }

    [Fact]
    public async Task SearchQuery_FiltersItemsByTitle()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", OwnerName = "Alice" },
            new Item { Id = 2, Title = "Ladder", OwnerName = "Bob" }
        };
        _mockItemRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Act
        _viewModel.SearchQuery = "drill";

        // Assert
        Assert.Single(_viewModel.Items);
        Assert.Equal("Drill", _viewModel.Items[0].Title);
    }

    [Fact]
    public async Task OwnerFilter_FiltersItemsByOwner()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", OwnerName = "Alice" },
            new Item { Id = 2, Title = "Ladder", OwnerName = "Bob" }
        };
        _mockItemRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Act
        _viewModel.SelectedOwnerFilter = "Alice";

        // Assert
        Assert.Single(_viewModel.Items);
        Assert.Equal("Drill", _viewModel.Items[0].Title);
    }

    [Fact]
    public async Task SortByPriceLowToHigh_SortsItemsCorrectly()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Ladder", OwnerName = "Alice", DailyRate = 20 },
            new Item { Id = 2, Title = "Drill", OwnerName = "Bob", DailyRate = 10 }
        };
        _mockItemRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Act
        _viewModel.SelectedSortOption = "Price: Low to High";

        // Assert
        Assert.Equal("Drill", _viewModel.Items[0].Title);
        Assert.Equal("Ladder", _viewModel.Items[1].Title);
    }

    [Fact]
    public async Task LoadItems_WhenRepositoryFails_SetsIsEmpty()
    {
        // Arrange
        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ThrowsAsync(new Exception("Network error"));

        // Act
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.IsEmpty);
    }

    [Fact]
    public async Task LoadItems_SetsIsBusyFalseAfterCompletion()
    {
        // Arrange
        _mockItemRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Item>());

        // Act
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsBusy);
    }
}