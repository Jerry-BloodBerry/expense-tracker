using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Expenses.Queries;

public record GetExpenseQuery(int Id) : IRequest<ErrorOr<ExpenseDto>>;

public class GetExpenseHandler : IRequestHandler<GetExpenseQuery, ErrorOr<ExpenseDto>>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetExpenseHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ErrorOr<ExpenseDto>> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
  {
    var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);

    if (expense is null)
      return ExpenseErrors.NotFound(request.Id);

    return new ExpenseDto
    {
      Id = expense.Id,
      Name = expense.Name,
      Amount = expense.Amount,
      Date = expense.Date,
      Description = expense.Description,
      Currency = expense.Currency,
      IsRecurring = expense.IsRecurring,
      RecurrenceInterval = expense.RecurrenceInterval,
      Category = new CategoryDto(expense.Category.Id),
      Tags = expense.Tags.Select(t => new TagDto(t.Id)).ToList()
    };
  }
}