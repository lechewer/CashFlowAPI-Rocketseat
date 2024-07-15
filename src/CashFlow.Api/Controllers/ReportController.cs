
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using CashFlow.Application.UseCases.Expenses.Reports.Excel;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf;

namespace CashFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    [HttpGet("Excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetExcel(
        [FromServices] IGenerateExpensesReportExcelUseCase useCase,
        [FromQuery] DateOnly month)
    {
        byte[] file = await useCase.Execute(month);
        
        if(file.Length > 0 )
            return File(file, MediaTypeNames.Application.Octet, $"Report_{month.ToString("MMMM")}.xlsx");

        return NoContent();
    }
    
    [HttpGet("Pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetPdf(
        [FromServices] IGenerateExpensesReportPdfUseCase useCase,
        [FromHeader] DateOnly month)
    {
        byte[] file = await useCase.Execute(month);

        if(file.Length > 0 )
            return File(file, MediaTypeNames.Application.Pdf, $"Report_{month.ToString("MMMM")}.pdf");

        return NoContent();
    }
}