using Application.Common.Models;
using MediatR;

namespace Application.Requests.Commands.SendQuotation;

public class SendQuotationItemDto
{
    public Guid ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}

public class SendQuotationCommand : IRequest<ApiResponse>
{
    public Guid RequestId { get; set; }
    public List<SendQuotationItemDto> Items { get; set; } = new();
}
