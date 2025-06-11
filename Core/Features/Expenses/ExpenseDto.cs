using Core.Domain;

namespace Features.Expenses;

public class ExpenseDto
{
  public int Id { get; set; }
  public string Name { get; set; }
  public CategoryDto Category { get; set; }
  public List<TagDto> Tags { get; set; } = new();
  public decimal Amount { get; set; }
  public DateTime Date { get; set; }
  public string Description { get; set; }
  public string Currency { get; set; }
  public bool IsRecurring { get; set; }
  public RecurrenceInterval? RecurrenceInterval { get; set; }
}

public class CategoryDto
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
}

public class TagDto
{
  public int Id { get; set; }
  public string Name { get; set; }
}