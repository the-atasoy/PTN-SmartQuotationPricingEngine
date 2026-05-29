using Application.Common.Models;
using MediatR;

namespace Application.Requests.Queries.GetQuotationRequestExcel;

public record GetQuotationRequestExcelQuery(Guid RequestId) : IRequest<ApiResponse<byte[]>>;
