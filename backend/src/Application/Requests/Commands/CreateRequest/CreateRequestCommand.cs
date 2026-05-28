using Application.Common.Models;
using Domain.Enums;
using MediatR;

namespace Application.Requests.Commands.CreateRequest;

public class CreateRequestItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateRequestCommand : IRequest<ApiResponse<Guid>>
{
    public string CustomerEmail { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public List<CreateRequestItemDto> Items { get; set; } = new();
}
