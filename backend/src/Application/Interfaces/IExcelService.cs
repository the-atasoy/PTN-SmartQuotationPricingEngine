using Domain.Entities;

namespace Application.Interfaces;

public interface IExcelService
{
    byte[] GenerateQuotationRequestExcel(Request request);
}
