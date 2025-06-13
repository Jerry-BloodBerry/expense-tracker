using Core.Entities;

namespace Core.Domain;

public class Tag : BaseEntity
{
  public string Name { get; private set; } = null!;

  private Tag() { }

  public Tag(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name cannot be empty", nameof(name));

    Name = name;
  }

  public void UpdateName(string newName)
  {
    if (string.IsNullOrWhiteSpace(newName))
      throw new ArgumentException("Name cannot be empty", nameof(newName));

    Name = newName;
  }
}