using Core.Domain;
using Core.Interfaces;
using MediatR;
using ErrorOr;

namespace Core.Features.Expenses.Commands;

public record DeleteExpenseCommand(int Id) : IRequest<ErrorOr<Unit>>;

public class DeleteExpenseHandler : IRequestHandler<DeleteExpenseCommand, ErrorOr<Unit>>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public DeleteExpenseHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<ErrorOr<Unit>> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
  {
    var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);
    if (expense is null)
      return ExpenseErrors.NotFound(request.Id);

    _expenseRepository.Delete(expense);
    await _expenseRepository.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}