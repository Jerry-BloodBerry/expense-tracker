using Core.Domain;
using Core.Features.Expenses.Specifications;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Expenses.Queries;

public class GetExpenseCategorySummaryQuery : IRequest<ErrorOr<ExpenseCategorySummaryResult>>
{
  public DateOnly StartDate { get; init; }
  public DateOnly EndDate { get; init; }
  public string Currency { get; init; } = string.Empty;
  public List<int> TagIds { get; init; } = [];
  public List<int> CategoryIds { get; init; } = [];
}

public class ExpenseCategorySummaryResult
{
  public string Currency { get; set; } = string.Empty;
  public DateOnly StartDate { get; set; }
  public DateOnly EndDate { get; set; }
  public List<CategorySummaryDataPoint> DataPoints { get; set; } = [];
}

public class CategorySummaryDataPoint
{
  public int CategoryId { get; set; }
  public string CategoryName { get; set; } = string.Empty;
  public decimal TotalAmount { get; set; }
  public int Count { get; set; }
}

public class GetExpenseCategorySummaryHandler : IRequestHandler<GetExpenseCategorySummaryQuery, ErrorOr<ExpenseCategorySummaryResult>>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetExpenseCategorySummaryHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ErrorOr<ExpenseCategorySummaryResult>> Handle(GetExpenseCategorySummaryQuery request, CancellationToken cancellationToken)
  {
    if (request.StartDate > request.EndDate)
    {
      return Error.Validation("StartDate cannot be after EndDate");
    }

    if (string.IsNullOrWhiteSpace(request.Currency))
    {
      return Error.Validation("Currency is required");
    }

    var spec = new GetExpenseCategorySummarySpecification(request);
    var expenses = await _expenseRepository.ListAsync(spec, cancellationToken);

    // Group expenses by category and sum amounts
    var categoryTotals = expenses
        .GroupBy(e => new { e.Category.Id, e.Category.Name })
        .Select(g => new CategorySummaryDataPoint
        {
          CategoryId = g.Key.Id,
          CategoryName = g.Key.Name,
          TotalAmount = g.Sum(e => e.Amount),
          Count = g.Count()
        })
        .OrderByDescending(dp => dp.TotalAmount)
        .ToList();

    return new ExpenseCategorySummaryResult
    {
      Currency = request.Currency,
      StartDate = request.StartDate,
      EndDate = request.EndDate,
      DataPoints = categoryTotals
    };
  }
}
