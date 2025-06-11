using Core.Domain;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Expenses.Commands;

public record DeleteExpenseCommand(int Id) : IRequest<Unit>;

public class DeleteExpenseHandler : IRequestHandler<DeleteExpenseCommand, Unit>
{
  private readonly IGenericRepository<Expense> _expenseRepository;

  public DeleteExpenseHandler(IGenericRepository<Expense> expenseRepository)
  {
    _expenseRepository = expenseRepository;
  }

  public async Task<Unit> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
  {
    var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Expense with ID {request.Id} not found");

    _expenseRepository.Delete(expense);
    await _expenseRepository.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}