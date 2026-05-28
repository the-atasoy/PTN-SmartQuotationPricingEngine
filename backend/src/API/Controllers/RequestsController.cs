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
}
