using ErrorOr;

namespace Core.Features.Tags;

public static class TagErrors
{
  public static Error NotFound(int id) =>
      Error.NotFound(
          code: "Tag.NotFound",
          description: $"Tag with ID {id} was not found.");

  public static Error NameAlreadyExists(string name) =>
      Error.Conflict(
          code: "Tag.NameAlreadyExists",
          description: $"A tag with the name '{name}' already exists.");
}