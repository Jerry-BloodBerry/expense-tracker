using Core.Domain;
using Core.Features.Expenses.Specifications;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Expenses.Queries;

public class GetExpenseReportQuery : IRequest<ErrorOr<ExpenseReportResult>>
{
  public DateTime StartDate { get; init; }
  public DateTime EndDate { get; init; }
  public string Currency { get; init; } = string.Empty;
  public List<int> TagIds { get; init; } = [];
  public List<int> CategoryIds { get; init; } = [];
}

public class ExpenseReportResult
{
  public string Currency { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public List<ExpenseReportDataPoint> DataPoints { get; set; } = [];
}

public class ExpenseReportDataPoint
{
  public DateTime Date { get; set; }
  public decimal Amount { get; set; }
}

public class GetExpenseReportHandler : IRequestHandler<GetExpenseReportQuery, ErrorOr<ExpenseReportResult>>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetExpenseReportHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ErrorOr<ExpenseReportResult>> Handle(GetExpenseReportQuery request, CancellationToken cancellationToken)
  {
    if (request.StartDate > request.EndDate)
    {
      return Error.Validation("StartDate cannot be after EndDate");
    }

    if (string.IsNullOrWhiteSpace(request.Currency))
    {
      return Error.Validation("Currency is required");
    }

    var spec = new GetExpenseReportSpecification(request);
    var expenses = await _expenseRepository.ListAsync(spec, cancellationToken);

    // Group expenses by date and sum amounts
    var dailyTotals = expenses
        .GroupBy(e => e.Date.Date)
        .Select(g => new ExpenseReportDataPoint
        {
          Date = g.Key,
          Amount = g.Sum(e => e.Amount)
        })
        .OrderBy(dp => dp.Date)
        .ToList();

    // Fill in missing dates with zero amounts
    var allDates = new List<ExpenseReportDataPoint>();
    var currentDate = request.StartDate.Date;

    while (currentDate <= request.EndDate.Date)
    {
      var existingDataPoint = dailyTotals.FirstOrDefault(dp => dp.Date == currentDate);
      allDates.Add(existingDataPoint ?? new ExpenseReportDataPoint
      {
        Date = currentDate,
        Amount = 0
      });
      currentDate = currentDate.AddDays(1);
    }

    return new ExpenseReportResult
    {
      Currency = request.Currency,
      StartDate = request.StartDate,
      EndDate = request.EndDate,
      DataPoints = allDates
    };
  }
}