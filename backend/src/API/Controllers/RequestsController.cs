using Application.Requests.Commands.CreateRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CreateRequestCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccessful)
        {
            return UnprocessableEntity(result);
        }

        return Ok(result);
    }

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 0, [FromQuery] int pageSize = 10, [FromQuery] string? sortColumn = null, [FromQuery] Application.Common.Enums.SortDirection? sortDirection = null)
    {
        var query = new Application.Requests.Queries.GetAllRequests.GetAllRequestsQuery 
        { 
            Page = page, 
            PageSize = pageSize,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new Application.Requests.Queries.GetRequestById.GetRequestByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccessful)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{id:guid}/send")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendQuotation(Guid id, [FromBody] Application.Requests.Commands.SendQuotation.SendQuotationCommand command)
    {
        if (id != command.RequestId)
            return BadRequest(Application.Common.Models.ApiResponse.Fail("ID mismatch", 400));

        var result = await _mediator.Send(command);

        if (!result.IsSuccessful)
        {
            if (result.StatusCode == 404) return NotFound(result);
            if (result.StatusCode == 409) return Conflict(result);
            return UnprocessableEntity(result);
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}/excel")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> DownloadExcel(Guid id)
    {
        var query = new Application.Requests.Queries.GetQuotationRequestExcel.GetQuotationRequestExcelQuery { RequestId = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccessful || result.Data == null)
            return NotFound(result);

        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Request_{id}.xlsx");
    }
}
