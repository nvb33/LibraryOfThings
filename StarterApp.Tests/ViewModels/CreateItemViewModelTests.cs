using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class CreateItemViewModelTests
{
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly CreateItemViewModel _viewModel;

    public CreateItemViewModelTests()
    {
        _mockItemRepository = new Mock<IItemRepository>();
        _viewModel = new CreateItemViewModel(_mockItemRepository.Object);
    }

    [Fact]
    public void Title_DefaultsToEmpty()
    {
        Assert.Equal(string.Empty, _viewModel.Title);
    }

    [Fact]
    public void Description_DefaultsToEmpty()
    {
        Assert.Equal(string.Empty, _viewModel.Description);
    }

    [Fact]
    public void DailyRate_DefaultsToZero()
    {
        Assert.Equal(0, _viewModel.DailyRate);
    }

    [Fact]
    public void SelectedCategory_DefaultsToNull()
    {
        Assert.Null(_viewModel.SelectedCategory);
    }

    [Fact]
    public async Task CreateItem_WhenTitleEmpty_SetsErrorMessage()
    {
        // Arrange
        _viewModel.Title = string.Empty;
        _viewModel.DailyRate = 10;
        _viewModel.SelectedCategory = new Category { Id = 1, Name = "Tools" };

        // Act
        await _viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Title is required.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CreateItem_WhenDailyRateZero_SetsErrorMessage()
    {
        // Arrange
        _viewModel.Title = "Drill";
        _viewModel.DailyRate = 0;
        _viewModel.SelectedCategory = new Category { Id = 1, Name = "Tools" };

        // Act
        await _viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Daily rate must be greater than zero.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CreateItem_WhenCategoryNull_SetsErrorMessage()
    {
        // Arrange
        _viewModel.Title = "Drill";
        _viewModel.DailyRate = 10;
        _viewModel.SelectedCategory = null;

        // Act
        await _viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Please select a category.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CreateItem_WhenValidInput_CallsRepository()
    {
        // Arrange
        _viewModel.Title = "Drill";
        _viewModel.DailyRate = 10;
        _viewModel.SelectedCategory = new Category { Id = 1, Name = "Tools" };
        _mockItemRepository
            .Setup(r => r.AddAsync(It.IsAny<Item>()))
            .ReturnsAsync((Item?)null);

        // Act
        await _viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        _mockItemRepository.Verify(r => r.AddAsync(
            It.Is<Item>(i => i.Title == "Drill" && i.DailyRate == 10)), Times.Once);
    }

    [Fact]
    public async Task LoadCategories_PopulatesCategoryList()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Tools" },
            new Category { Id = 2, Name = "Sports" }
        };
        _mockItemRepository.Setup(r => r.GetCategoriesAsync()).ReturnsAsync(categories);

        // Act
        await _viewModel.LoadCategoriesCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, _viewModel.Categories.Count);
    }

    [Fact]
    public async Task CreateItem_SetsIsBusyFalseAfterCompletion()
    {
        // Arrange
        _viewModel.Title = "Drill";
        _viewModel.DailyRate = 10;
        _viewModel.SelectedCategory = new Category { Id = 1, Name = "Tools" };
        _mockItemRepository
            .Setup(r => r.AddAsync(It.IsAny<Item>()))
            .ReturnsAsync((Item?)null);

        // Act
        await _viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsBusy);
    }
}