using Domain.Enums;

namespace Application.Requests.Queries.GetAllRequests;

public class RequestListItemDto
{
    public Guid Id { get; set; }
    public string RequestNo { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public Currency Currency { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}
