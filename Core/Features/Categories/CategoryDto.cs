namespace Core.Features.Categories;

public class CategoryDto
{
  public int Id { get; init; }
  public string Name { get; init; } = string.Empty;
  public string? Description { get; init; }
}