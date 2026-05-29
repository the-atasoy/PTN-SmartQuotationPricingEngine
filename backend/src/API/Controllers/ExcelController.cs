using Application.Excel.Commands.ParseUploadedExcel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Application.Resources;
using Asp.Versioning;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExcelController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> _localizer;

    public ExcelController(IMediator mediator, Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpPost("parse")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ParseExcel(IFormFile file, [FromForm] Guid requestId)
    {
        if (file.Length == 0)
        {
            return BadRequest(Application.Common.Models.ApiResponse.Fail(_localizer["NoFileUploaded"].Value, 400));
        }

        if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".xlsx")
        {
            return BadRequest(Application.Common.Models.ApiResponse.Fail(_localizer["InvalidFileType"].Value, 400));
        }

        using var stream = file.OpenReadStream();
        var command = new ParseUploadedExcelCommand { ExcelStream = stream, RequestId = requestId };
        var result = await _mediator.Send(command);

        if (!result.IsSuccessful)
        {
            if (result.StatusCode == 422) return UnprocessableEntity(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}
