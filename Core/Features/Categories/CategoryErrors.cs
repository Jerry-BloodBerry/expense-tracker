using ErrorOr;

namespace Core.Features.Categories;

public static class CategoryErrors
{
  public static Error NotFound(int id) =>
    Error.NotFound(
      code: "Category.NotFound",
      description: $"Category with ID {id} not found");
}