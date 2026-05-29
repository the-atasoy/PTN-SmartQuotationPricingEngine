using Application.Common.Models;
using MediatR;

namespace Application.Requests.Queries.GetQuotationRequestExcel;

public class GetQuotationRequestExcelQuery : IRequest<ApiResponse<byte[]>>
{
    public Guid RequestId { get; set; }
}
