namespace API.Features.Categories;

public class CategoryResponse
{
  public int Id { get; init; }
  public string Name { get; init; } = string.Empty;
  public string? Description { get; init; }
}