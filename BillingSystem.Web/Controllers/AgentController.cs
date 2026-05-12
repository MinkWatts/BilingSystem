using BillingSystem.Handlers.Commands;
using BillingSystem.Handlers.Queries;
using BillingSystem.Models.DTOs;
using BillingSystem.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingSystem.Web.Controllers
{
    [Authorize(Roles = "Agent")]
    public class AgentController : Controller
    {
        private readonly IMediator _mediator;
        private readonly InvoicePdfService _pdfService;

        public AgentController(IMediator mediator, InvoicePdfService pdfService)
        {
            _mediator = mediator;
            _pdfService = pdfService;
        }

        // GET: /Agent/GenerateBill → redirects to Invoice page
        public IActionResult GenerateBill()
        {
            return RedirectToAction("Invoice");
        }

        // POST: /Agent/GenerateBill → accepts JSON body, saves bill, returns redirect URL
        [HttpPost]
        public async Task<IActionResult> GenerateBill([FromBody] CreateBillDto model)
        {
            if (model == null)
                return BadRequest(new { error = "Invalid request body." });

            if (model.Items == null || !model.Items.Any())
                return BadRequest(new { error = "Bill must contain at least one item." });

            if (string.IsNullOrWhiteSpace(model.CustomerName))
                return BadRequest(new { error = "Customer name is required." });

            model.AgentId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            model.CreatedBy = User.FindFirstValue(ClaimTypes.Email) ?? "System";

            var invoiceId = await _mediator.Send(new GenerateBillCommand { BillDto = model });

            if (invoiceId > 0)
                return Json(new { redirectUrl = Url.Action("Invoice", new { id = invoiceId }) });

            return StatusCode(500, new { error = "Failed to generate bill. Please check server logs." });
        }

        // GET: /Agent/Invoice      → blank new invoice form
        // GET: /Agent/Invoice/5    → saved invoice view
        public async Task<IActionResult> Invoice(int? id = null)
        {
            if (id.HasValue && id > 0)
            {
                var invoice = await _mediator.Send(new GetInvoiceQuery { InvoiceId = id.Value });
                if (invoice == null || invoice.Id == 0)
                    return RedirectToAction("Invoice");
                return View(invoice);
            }
            return View();
        }

        // GET: /Agent/GetProducts → returns product list as JSON for dropdown
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _mediator.Send(new GetProductsQuery());
            return Json(products);
        }

        // GET: /Agent/DownloadPdf/5 → generates and streams PDF
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