using Core.Domain;
using Core.Features.Expenses.Queries;
using ErrorOr;

namespace Core.Features.Expenses;

public static class ExpenseErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound(
            code: "Expense.NotFound",
            description: $"Expense with ID {id} not found");

    public static Error CategoryNotFound(int id) =>
        Error.NotFound(
            code: "Expense.CategoryNotFound",
            description: $"Category with ID {id} not found");

    public static Error TagNotFound(int id) =>
        Error.NotFound(
            code: "Expense.TagNotFound",
            description: $"Tag with ID {id} not found");

    public static Error Validation(string message) =>
        Error.Validation(
            code: "Expense.Validation",
            description: message);

    public static Error InvalidRecurrenceInterval(string recurrenceInterval) =>
        Error.Validation(
            code: "Expense.InvalidRecurrenceInterval",
            description: $"{recurrenceInterval} is invalid. Valid intervals: " + string.Join(", ", Enum.GetNames<RecurrenceInterval>())
        );
}