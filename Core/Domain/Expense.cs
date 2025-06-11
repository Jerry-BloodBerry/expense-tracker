using Core.Entities;

namespace Core.Domain;

public class Expense : BaseEntity
{
    private readonly List<Tag> _tags;
    public string Name { get; private set; }
    public Category Category { get; private set; }
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }
    public string Currency { get; private set; }
    public bool IsRecurring { get; private set; }
    public RecurrenceInterval? RecurrenceInterval { get; private set; }

    private Expense()
    {
        _tags = new List<Tag>();
    }

    public Expense(string name, Category category, decimal amount, DateTime date, string currency)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (category == null)
            throw new ArgumentNullException(nameof(category));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        Name = name;
        Category = category;
        Amount = amount;
        Date = date;
        Currency = currency;
        _tags = new List<Tag>();
    }

    public void UpdateAmount(decimal newAmount)
    {
        if (newAmount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(newAmount));

        Amount = newAmount;
    }

    public void UpdateCategory(Category newCategory)
    {
        ArgumentNullException.ThrowIfNull(newCategory);
        Category = newCategory;
    }

    public void AddTag(Tag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);

        if (!_tags.Contains(tag))
            _tags.Add(tag);
    }

    public void RemoveTag(Tag tag)
    {
        _tags.Remove(tag);
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void SetAsRecurring(RecurrenceInterval interval)
    {
        IsRecurring = true;
        RecurrenceInterval = interval;
    }

    public void RemoveRecurrence()
    {
        IsRecurring = false;
        RecurrenceInterval = null;
    }

    public Expense CreateRecurrence(DateTime newDate)
    {
        if (!IsRecurring)
            throw new InvalidOperationException("Cannot create recurrence for non-recurring expense");

        return new Expense
        {
            Name = this.Name,
            Category = this.Category,
            Amount = this.Amount,
            Date = newDate,
            Description = this.Description,
            Currency = this.Currency,
            IsRecurring = true,
            RecurrenceInterval = this.RecurrenceInterval
        };
    }
}