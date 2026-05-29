using Application.Common.Models;
using MediatR;

namespace Application.Excel.Commands.ParseUploadedExcel;

public class ParseUploadedExcelCommand : IRequest<ApiResponse<List<ParsedExcelResultDto>>>
{
    public Stream ExcelStream { get; set; } = default!;
}
