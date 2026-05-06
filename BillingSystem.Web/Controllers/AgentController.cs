using BillingSystem.Handlers.Commands;
using BillingSystem.Handlers.Queries;
using BillingSystem.Models.DTOs;
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

        public AgentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Agent/GenerateBill → redirects directly to Invoice page
        public IActionResult GenerateBill()
        {
            return RedirectToAction("Invoice");
        }

        // POST: /Agent/GenerateBill → saves bill then shows invoice
        [HttpPost]
        public async Task<IActionResult> GenerateBill(CreateBillDto model)
        {
            model.AgentId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? string.Empty;
            model.CreatedBy = User.FindFirstValue(ClaimTypes.Email)
                ?? "System";

            var invoiceId = await _mediator.Send(new GenerateBillCommand
            {
                BillDto = model
            });

            if (invoiceId > 0)
                return RedirectToAction("Invoice", new { id = invoiceId });

            ModelState.AddModelError("", "Failed to generate bill");
            return RedirectToAction("Invoice");
        }

        // GET: /Agent/Invoice      → blank new invoice with dropdowns
        // GET: /Agent/Invoice/5    → saved invoice view
        public async Task<IActionResult> Invoice(int? id = null)
        {
            if (id.HasValue && id > 0)
            {
                var invoice = await _mediator.Send(new GetInvoiceQuery
                {
                    InvoiceId = id.Value
                });
                return View(invoice);
            }

            // New blank invoice
            return View();
        }

        // GET: /Agent/GetProducts → returns JSON for dropdown
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _mediator.Send(new GetProductsQuery());
            return Json(products);
        }
    }
}