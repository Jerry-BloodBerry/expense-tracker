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