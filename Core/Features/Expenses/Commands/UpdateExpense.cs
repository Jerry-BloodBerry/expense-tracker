using Core.Domain;
using Core.Features.Tags.Specifications;
using Core.Interfaces;
using MediatR;

namespace Core.Features.Expenses.Commands;

public record UpdateExpenseCommand : IRequest<Unit>
{
  public int Id { get; init; }
  public string Name { get; init; }
  public int CategoryId { get; init; }
  public decimal Amount { get; init; }
  public DateTime Date { get; init; }
  public string Description { get; init; }
  public string Currency { get; init; }
  public bool IsRecurring { get; init; }
  public RecurrenceInterval? RecurrenceInterval { get; init; }
  public List<int> TagIds { get; init; } = new();
}

public class UpdateExpenseHandler : IRequestHandler<UpdateExpenseCommand, Unit>
{
  private readonly IGenericRepository<Expense> _expenseRepository;
  private readonly IGenericRepository<Category> _categoryRepository;
  private readonly IGenericRepository<Tag> _tagRepository;

  public UpdateExpenseHandler(
      IGenericRepository<Expense> expenseRepository,
      IGenericRepository<Category> categoryRepository,
      IGenericRepository<Tag> tagRepository)
  {
    _expenseRepository = expenseRepository;
    _categoryRepository = categoryRepository;
    _tagRepository = tagRepository;
  }

  public async Task<Unit> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
  {
    var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken)
    ?? throw new NotFoundException($"Expense with ID {request.Id} not found");

    var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
        ?? throw new NotFoundException($"Category with ID {request.CategoryId} not found");

    expense.UpdateCategory(category);
    expense.UpdateAmount(request.Amount);
    expense.SetDescription(request.Description);

    if (request.IsRecurring && request.RecurrenceInterval.HasValue)
    {
      expense.SetAsRecurring(request.RecurrenceInterval.Value);
    }
    else
    {
      expense.RemoveRecurrence();
    }

    // Update tags
    var currentTags = expense.Tags.ToList();
    var newTags = await _tagRepository.ListAsync(new TagsWithIdsSpecification(request.TagIds), cancellationToken);

    // Remove tags that are no longer present
    foreach (var tag in currentTags)
    {
      if (!request.TagIds.Contains(tag.Id))
      {
        expense.RemoveTag(tag);
      }
    }

    // Add new tags
    foreach (var tag in newTags)
    {
      if (!currentTags.Any(t => t.Id == tag.Id))
      {
        expense.AddTag(tag);
      }
    }

    await _expenseRepository.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}