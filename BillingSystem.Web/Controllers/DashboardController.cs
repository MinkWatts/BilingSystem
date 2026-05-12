using BillingSystem.Handlers.Queries;
using BillingSystem.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;
        private readonly CsvExportService _csvService;
        private readonly InvoicePdfService _pdfService;

        public DashboardController(IMediator mediator, CsvExportService csvService, InvoicePdfService pdfService)
        {
            _mediator = mediator;
            _csvService = csvService;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _mediator.Send(new GetDashboardQuery());
            return View(data);
        }

        public async Task<IActionResult> Reports()
        {
            var data = await _mediator.Send(new GetDashboardQuery());
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            var data = await _mediator.Send(new GetDashboardQuery());
            var csvBytes = _csvService.ExportBillsToCsv(data.RecentBills);
            var fileName = $"Bills-Export-{DateTime.Now:yyyyMMdd-HHmm}.csv";
            return File(csvBytes, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var invoice = await _mediator.Send(new GetInvoiceQuery { InvoiceId = id });
            if (invoice == null || invoice.Id == 0)
                return NotFound();

            var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Invoice-{invoice.InvoiceNumber}.pdf");
        }
    }
}