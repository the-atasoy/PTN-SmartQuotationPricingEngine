using Domain.Enums;

namespace Application.Requests.Queries.GetRequestById;

public class RequestItemDetailDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
    public decimal Discount { get; init; }
    public decimal BasePrice { get; init; }
    public decimal? LastRequestPrice { get; init; }
    public Currency? LastRequestCurrency { get; init; }
    public DateTime? LastRequestDate { get; init; }
}

public class RequestDetailDto
{
    public Guid Id { get; init; }
    public string RequestNo { get; init; } = default!;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = default!;
    public string CustomerEmail { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public Currency Currency { get; init; }
    public RequestStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModified { get; init; }
    
    public List<RequestItemDetailDto> Items { get; init; } = new();
}
