using Domain.Enums;

namespace Application.Requests.Queries.GetAllRequests;

public class RequestListItemDto
{
    public Guid Id { get; init; }
    public string RequestNo { get; init; } = default!;
    public string CustomerName { get; init; } = default!;
    public string CustomerEmail { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public Currency Currency { get; init; }
    public RequestStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModified { get; init; }
}
