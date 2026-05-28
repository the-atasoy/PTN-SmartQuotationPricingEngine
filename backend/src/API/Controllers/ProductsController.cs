using Application.Features.Products.Queries.GetAllProducts;
using Application.Features.Products.Queries.GetPriceHistory;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAllProducts([FromQuery] int page = 0, [FromQuery] int pageSize = 10, [FromQuery] string? sortColumn = null, [FromQuery] Application.Common.Enums.SortDirection? sortDirection = null)
    {
        var query = new GetAllProductsQuery(page, pageSize, sortColumn, sortDirection);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    [HttpGet("{id}/price-history")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPriceHistory(Guid id, [FromQuery] int page = 0, [FromQuery] int pageSize = 10, [FromQuery] string? sortColumn = null, [FromQuery] Application.Common.Enums.SortDirection? sortDirection = null)
    {
        var query = new GetPriceHistoryQuery(id, page, pageSize, sortColumn, sortDirection);
        var result = await _mediator.Send(query);

        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }
}
