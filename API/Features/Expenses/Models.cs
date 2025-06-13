namespace API.Features.Expenses;

public record ExpenseResponse(
  int Id,
  string Name,
  int Category,
  List<int> Tags,
  decimal Amount,
  DateTime Date,
  string? Description,
  string Currency,
  bool IsRecurring,
  string? RecurrenceInterval);

public static class ExpenseDtoExtensions
{
  public static ExpenseResponse AsResponse(this Core.Features.Expenses.Queries.ExpenseDto dto) => new(
      Id: dto.Id,
      Name: dto.Name,
      Category: dto.Category.Id,
      Tags: dto.Tags.Select(t => t.Id).ToList(),
      Amount: dto.Amount,
      Date: dto.Date,
      Description: dto.Description,
      Currency: dto.Currency,
      IsRecurring: dto.IsRecurring,
      RecurrenceInterval: dto.RecurrenceInterval?.ToString());
}