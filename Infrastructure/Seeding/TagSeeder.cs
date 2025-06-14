using System.Text.Json;
using Core.Domain;
using Core.Interfaces;

namespace Infrastructure.Seeding;

public class TagSeeder : ISeeder
{
  private readonly IGenericRepository<Tag> _tagRepository;
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public TagSeeder(IGenericRepository<Tag> tagRepository)
  {
    _tagRepository = tagRepository;
  }

  public async Task SeedAsync()
  {
    if (_tagRepository.Exists(1))
    {
      return;
    }

    var tagsJson = await File.ReadAllTextAsync("../Infrastructure/Seeding/SeedData/tags.json");
    var tags = JsonSerializer.Deserialize<List<TagSeedData>>(tagsJson, _jsonOptions)!;

    foreach (var tagData in tags)
    {
      var tag = new Tag(tagData.Name);
      _tagRepository.Add(tag);
    }

    await _tagRepository.SaveChangesAsync(CancellationToken.None);
  }

  public sealed class TagSeedData
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
  }
}