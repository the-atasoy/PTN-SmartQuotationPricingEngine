using Domain.Enums;

namespace Application.Requests.Queries.GetRequestById;

public class RequestItemDetailDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public decimal? LastRequestPrice { get; set; }
    public DateTime? LastRequestDate { get; set; }
}

public class RequestDetailDto
{
    public Guid Id { get; set; }
    public string RequestNo { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public Currency Currency { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    
    public List<RequestItemDetailDto> Items { get; set; } = new();
}
