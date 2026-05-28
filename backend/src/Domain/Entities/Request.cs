using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Aggregate Root — owns RequestItem collection.
/// All modifications to items must go through this entity.
/// </summary>
public class Request : BaseEntity
{
    public string RequestNo { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Currency Currency { get; private set; }
    public RequestStatus Status { get; private set; }
    public DateTime? SentAt { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = default!;

    private readonly List<RequestItem> _items = [];
    public IReadOnlyCollection<RequestItem> Items => _items.AsReadOnly();

    private Request() { } // EF Core

    public static Request Create(string requestNo, Guid customerId, Currency currency)
    {
        if (string.IsNullOrWhiteSpace(requestNo))
            throw new ArgumentException("Request number is required.", nameof(requestNo));

        return new Request
        {
            RequestNo = requestNo.Trim(),
            CustomerId = customerId,
            TotalAmount = 0,
            Currency = currency,
            Status = RequestStatus.Pending
        };
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice, decimal discount = 0)
    {
        var item = RequestItem.Create(Id, productId, quantity, unitPrice, discount);
        _items.Add(item);
        RecalculateTotal();
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException($"Item with id '{itemId}' not found in this request.");

        _items.Remove(item);
        RecalculateTotal();
    }

    public void UpdateItem(Guid itemId, int quantity, decimal unitPrice, decimal discount)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException($"Item with id '{itemId}' not found in this request.");

        item.Update(quantity, unitPrice, discount);
        RecalculateTotal();
    }

    public void MarkAsSent()
    {
        if (Status == RequestStatus.Cancelled)
            throw new InvalidOperationException("Cannot send a cancelled request.");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot send a request with no items.");

        Status = RequestStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == RequestStatus.Sent)
            throw new InvalidOperationException("Cannot cancel an already sent request.");

        Status = RequestStatus.Cancelled;
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.LineTotal);
    }
}
