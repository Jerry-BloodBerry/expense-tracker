using Core.Domain;
using Core.Features.Expenses.Specifications;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Expenses.Queries;

public record GetExpenseQuery(int Id) : IRequest<ExpenseDto>;

public class GetExpenseHandler : IRequestHandler<GetExpenseQuery, ExpenseDto>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public GetExpenseHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ExpenseDto> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
  {
    var spec = new GetExpenseByIdSpecification(request.Id);
    var expense = await _expenseRepository.GetEntityWithSpecAsync(spec, cancellationToken)
        ?? throw new NotFoundException($"Expense with ID {request.Id} not found");

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