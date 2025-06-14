using Core.Domain;
using Core.Interfaces;
using FastEndpoints;

namespace API.Features.Expenses;

public record GetExpenseRequest
{
  public int Id { get; init; }
}

public class GetExpenseEndpoint : Endpoint<GetExpenseRequest, ExpenseResponse>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetExpenseEndpoint(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public override void Configure()
  {
    Get("/api/expenses/{id}");
    AllowAnonymous();
    Description(d => d
        .WithSummary("Get expense by ID")
        .Produces<ExpenseResponse>(200)
        .ProducesProblem(404)
        .WithTags("Expenses"));
  }

  public override async Task HandleAsync(GetExpenseRequest req, CancellationToken ct)
  {
    var expense = await _expenseRepository.GetByIdAsync(req.Id, ct);

    if (expense is null)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    var response = new ExpenseResponse(
      Id: expense.Id,
      Name: expense.Name,
      Amount: expense.Amount,
      Date: expense.Date,
      Description: expense.Description,
      Currency: expense.Currency,
      IsRecurring: expense.IsRecurring,
      RecurrenceInterval: expense.RecurrenceInterval?.ToString(),
      Category: expense.Category.Id,
      Tags: expense.Tags.Select(t => t.Id).ToList()
    );

    await SendOkAsync(response, ct);
  }
}