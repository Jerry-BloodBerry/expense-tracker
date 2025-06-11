using Core.Domain;

namespace API.Features.Expenses;

public class ExpensesResult
{
  public List<ExpenseResponse> Items { get; set; } = new();
  public int TotalCount { get; set; }
  public int Page { get; set; }
  public int PageSize { get; set; }
  public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class ExpenseResponse
{
  public int Id { get; set; }
  public string Name { get; set; }
  public CategoryResponse Category { get; set; }
  public List<TagResponse> Tags { get; set; } = new();
  public decimal Amount { get; set; }
  public DateTime Date { get; set; }
  public string Description { get; set; }
  public string Currency { get; set; }
  public bool IsRecurring { get; set; }
  public RecurrenceInterval? RecurrenceInterval { get; set; }
}

public class CategoryResponse
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
}

public class TagResponse
{
  public int Id { get; set; }
  public string Name { get; set; }
}