using Core.Entities;

namespace Core.Domain;

public class Category : BaseEntity
{
  public string Name { get; private set; } = null!;
  public string? Description { get; private set; }

  private Category() { }

  public Category(string name, string? description = null)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name cannot be empty", nameof(name));

    Name = name;
    Description = description;
  }

  public void UpdateName(string newName)
  {
    if (string.IsNullOrWhiteSpace(newName))
      throw new ArgumentException("Name cannot be empty", nameof(newName));

    Name = newName;
  }

  public void UpdateDescription(string? description)
  {
    Description = description;
  }
}