using Core.Domain;

namespace Core.Features.Expenses.Queries;

public record ExpenseDto
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

public record CategoryDto(int Id);

public record TagDto(int Id);