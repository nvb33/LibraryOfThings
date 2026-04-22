using Moq;
using StarterApp.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Tests;

/// <summary>
/// Unit tests for ItemRepository verifying correct delegation to IApiService.
/// </summary>
public class ItemRepositoryTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly ItemRepository _repository;

    public ItemRepositoryTests()
    {
        _mockApiService = new Mock<IApiService>();
        _repository = new ItemRepository(_mockApiService.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsItemsFromApiService()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill" },
            new Item { Id = 2, Title = "Ladder" }
        };
        _mockApiService.Setup(s => s.GetItemsAsync()).ReturnsAsync(items);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectItem()
    {
        // Arrange
        var item = new Item { Id = 1, Title = "Drill" };
        _mockApiService.Setup(s => s.GetItemAsync(1)).ReturnsAsync(item);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Drill", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _mockApiService.Setup(s => s.GetItemAsync(99)).ReturnsAsync((Item?)null);

        // Act
        var result = await _repository.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_CreatesItemViaApiService()
    {
        // Arrange
        var item = new Item { Title = "Tent", DailyRate = 10.00m };
        var created = new Item { Id = 5, Title = "Tent", DailyRate = 10.00m };
        _mockApiService.Setup(s => s.CreateItemAsync(item)).ReturnsAsync(created);

        // Act
        var result = await _repository.AddAsync(item);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrueWhenSuccessful()
    {
        // Arrange
        var item = new Item { Id = 1, Title = "Updated Drill" };
        _mockApiService.Setup(s => s.UpdateItemAsync(1, item)).ReturnsAsync(item);

        // Act
        var result = await _repository.UpdateAsync(1, item);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalseWhenApiFails()
    {
        // Arrange
        var item = new Item { Id = 1, Title = "Updated Drill" };
        _mockApiService.Setup(s => s.UpdateItemAsync(1, item)).ReturnsAsync((Item?)null);

        // Act
        var result = await _repository.UpdateAsync(1, item);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_AlwaysReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetNearbyAsync_ReturnsNearbyItemsFromApiService()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", Distance = 1.2 }
        };
        _mockApiService
            .Setup(s => s.GetNearbyItemsAsync(55.9533, -3.1883, 5))
            .ReturnsAsync(items);

        // Act
        var result = await _repository.GetNearbyAsync(55.9533, -3.1883, 5);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetCategoriesAsync_ReturnsCategoriesFromApiService()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Tools" },
            new Category { Id = 2, Name = "Sports" }
        };
        _mockApiService.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(categories);

        // Act
        var result = await _repository.GetCategoriesAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}