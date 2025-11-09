using System.Linq;
using Core.Domain;
using Core.Features.Expenses.Specifications;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Expenses.Queries;

public class GetTotalRecurringExpensesThisMonthQuery : IRequest<ErrorOr<decimal>>
{
  public DateTime StartDate { get; init; }
  public DateTime EndDate { get; init; }
}

public class GetTotalRecurringExpensesThisMonthHandler : IRequestHandler<GetTotalRecurringExpensesThisMonthQuery, ErrorOr<decimal>>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetTotalRecurringExpensesThisMonthHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ErrorOr<decimal>> Handle(GetTotalRecurringExpensesThisMonthQuery request, CancellationToken cancellationToken)
  {
    var spec = new GetAllExpensesSummarySpecification(request.StartDate, request.EndDate, isRecurring: true);
    var expenses = await _expenseRepository.ListAsync(spec, cancellationToken);

    var total = expenses.Sum(e => e.Amount);
    return total;
  }
}

