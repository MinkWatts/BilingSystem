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

        // GET: /Agent/GenerateBill
        public async Task<IActionResult> GenerateBill()
        {
            var products = await _mediator
                .Send(new GetProductsQuery());
            return View(products);
        }

        // POST: /Agent/GenerateBill
        [HttpPost]
        public async Task<IActionResult> GenerateBill(
            CreateBillDto model)
        {
            model.AgentId = User.FindFirstValue(
                ClaimTypes.NameIdentifier)
                ?? string.Empty;

            model.CreatedBy = User.FindFirstValue(
                ClaimTypes.Email) ?? "System";

            var invoiceId = await _mediator.Send(
                new GenerateBillCommand
                {
                    BillDto = model
                });

            if (invoiceId > 0)
                return RedirectToAction(
                    "Invoice", new { id = invoiceId });

            ModelState.AddModelError("",
                "Failed to generate bill");
            return RedirectToAction("GenerateBill");
        }

        // GET: /Agent/Invoice/5
        public async Task<IActionResult> Invoice(int id)
        {
            var invoice = await _mediator
                .Send(new GetInvoiceQuery
                {
                    InvoiceId = id
                });

            return View(invoice);
        }
    }
}