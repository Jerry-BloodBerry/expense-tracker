using Core.Domain;
using Core.Specifications;

namespace Core.Features.Expenses.Specifications;

public class GetExpenseByIdSpecification : BaseSpecification<Expense>
{
  public GetExpenseByIdSpecification(int id)
      : base(e => e.Id == id)
  {
    AddInclude(e => e.Tags);
    AddInclude(e => e.Category);
  }
}