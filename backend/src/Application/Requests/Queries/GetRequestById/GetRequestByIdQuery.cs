using Application.Common.Models;
using MediatR;

namespace Application.Requests.Queries.GetRequestById;

public class GetRequestByIdQuery : IRequest<ApiResponse<RequestDetailDto>>
{
    public Guid Id { get; set; }
}
