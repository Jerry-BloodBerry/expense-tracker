namespace API.Features.Expenses;

public record ExpenseResponse(
  int Id,
  string Name,
  CategoryResponse Category,
  List<TagResponse> Tags,
  decimal Amount,
  DateTime Date,
  string? Description,
  string Currency,
  bool IsRecurring,
  string? RecurrenceInterval);

public record CategoryResponse(
  int Id,
  string Name,
  string? Description);

public record TagResponse(
  int Id,
  string Name);

public static class ExpenseDtoExtensions
{
  public static ExpenseResponse AsResponse(this Core.Features.Expenses.Queries.ExpenseDto dto) => new(
      Id: dto.Id,
      Name: dto.Name,
      Category: new CategoryResponse(
          Id: dto.Category.Id,
          Name: dto.Category.Name,
          Description: dto.Category.Description),
      Tags: dto.Tags.Select(t => new TagResponse(
          Id: t.Id,
          Name: t.Name)).ToList(),
      Amount: dto.Amount,
      Date: dto.Date,
      Description: dto.Description,
      Currency: dto.Currency,
      IsRecurring: dto.IsRecurring,
      RecurrenceInterval: dto.RecurrenceInterval?.ToString());
}