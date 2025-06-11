using Core.Domain;

namespace Features.Expenses.Queries;

public class ExpenseDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public DateTime Date { get; set; }
  public string? Description { get; set; }
  public string Currency { get; set; } = string.Empty;
  public bool IsRecurring { get; set; }
  public RecurrenceInterval? RecurrenceInterval { get; set; }
  public CategoryDto Category { get; set; } = null!;
  public List<TagDto> Tags { get; set; } = new();
}

public class CategoryDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
}

public class TagDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
}