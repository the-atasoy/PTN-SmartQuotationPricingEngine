using Application.Excel.Commands.ParseUploadedExcel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ExcelController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExcelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("parse")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ParseExcel(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest(Application.Common.Models.ApiResponse.Fail("No file uploaded", 400));
        }

        if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".xlsx")
        {
            return BadRequest(Application.Common.Models.ApiResponse.Fail("Invalid file type. Only .xlsx is allowed.", 400));
        }

        using var stream = file.OpenReadStream();
        var command = new ParseUploadedExcelCommand { ExcelStream = stream };
        var result = await _mediator.Send(command);

        if (!result.IsSuccessful)
        {
            if (result.StatusCode == 422) return UnprocessableEntity(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}
