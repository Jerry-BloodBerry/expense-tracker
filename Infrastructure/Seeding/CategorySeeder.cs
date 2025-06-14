using System.Text.Json;
using Core.Domain;
using Core.Interfaces;

namespace Infrastructure.Seeding;

public class CategorySeeder : ISeeder
{
  private readonly IGenericRepository<Category> _categoryRepository;
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public CategorySeeder(IGenericRepository<Category> categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task SeedAsync()
  {
    if (_categoryRepository.Exists(1))
    {
      return;
    }

    var categoriesJson = await File.ReadAllTextAsync("../Infrastructure/Seeding/SeedData/categories.json");
    var categories = JsonSerializer.Deserialize<List<CategorySeedData>>(categoriesJson, _jsonOptions)!;

    foreach (var categoryData in categories)
    {
      var category = new Category(categoryData.Name, categoryData.Description);
      _categoryRepository.Add(category);
    }

    await _categoryRepository.SaveChangesAsync(CancellationToken.None);
  }

  public sealed class CategorySeedData
  {
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
  }
}