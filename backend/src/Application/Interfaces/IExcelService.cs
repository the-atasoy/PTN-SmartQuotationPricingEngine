using Application.Excel.Commands.ParseUploadedExcel;
using Domain.Entities;

namespace Application.Interfaces;

public interface IExcelService
{
    byte[] GenerateQuotationRequestExcel(Request request);
    List<ParsedExcelResultDto> ParseExcel(Stream stream);
}
