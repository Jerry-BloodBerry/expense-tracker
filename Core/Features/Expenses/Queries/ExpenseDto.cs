using Core.Domain;

namespace Core.Features.Expenses.Queries;

public class ExpenseDto
{
  public required int Id { get; init; }
  public required string Name { get; init; }
  public required decimal Amount { get; init; }
  public DateTime Date { get; init; }
  public string? Description { get; init; }
  public required string Currency { get; init; }
  public bool IsRecurring { get; init; }
  public RecurrenceInterval? RecurrenceInterval { get; init; }
  public required CategoryDto Category { get; init; }
  public List<TagDto> Tags { get; init; } = [];
}

public class CategoryDto
{
  public required int Id { get; init; }
  public required string Name { get; init; }
  public string? Description { get; set; }
}

public class TagDto
{
  public required int Id { get; init; }
  public required string Name { get; init; }
}