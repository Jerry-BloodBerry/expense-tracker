using Core.Domain;
using Core.Features.Expenses.Queries;
using Core.Specifications;

namespace Core.Features.Expenses.Specifications;

public class GetAllExpensesSummarySpecification : BaseSpecification<Expense>
{
  public GetAllExpensesSummarySpecification(DateTime startDate, DateTime endDate, bool? isRecurring = null)
      : base(e =>
          e.Date >= startDate &&
          e.Date <= endDate &&
          (!isRecurring.HasValue || e.IsRecurring == isRecurring))
  {
    AddOrderByDescending(e => e.Date);
    // No paging - we want all expenses for summary calculations
  }
}

