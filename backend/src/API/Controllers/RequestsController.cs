using Application.Requests.Commands.CreateRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

using Application.Resources;
using Microsoft.Extensions.Localization;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public RequestsController(IMediator mediator, IStringLocalizer<SharedResource> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpPost]
    [Authorize]
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
    [Authorize(Roles = "Admin")]
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
        
        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new Application.Requests.Queries.GetRequestById.GetRequestByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}/send")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendQuotation(Guid id, [FromBody] Application.Requests.Commands.SendQuotation.SendQuotationCommand command)
    {
        if (id != command.RequestId)
            return BadRequest(Application.Common.Models.ApiResponse.Fail(_localizer["IdMismatch"].Value, 400));

        var result = await _mediator.Send(command);

        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}/excel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DownloadExcel(Guid id)
    {
        var query = new Application.Requests.Queries.GetQuotationRequestExcel.GetQuotationRequestExcelQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccessful || result.Data == null)
        {
            return StatusCode(result.StatusCode, result);
        }

        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Request_{id}.xlsx");
    }
}
