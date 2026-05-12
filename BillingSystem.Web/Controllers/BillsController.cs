using Microsoft.AspNetCore.Mvc;
using MediatR;
using BillingSystem.Handlers.Queries;
using BillingSystem.Handlers.Commands;
using Microsoft.AspNetCore.Authorization;
using BillingSystem.Models.ViewModels;
using System.Security.Claims;

namespace BillingSystem.Web.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly IMediator _mediator;

        public BillsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Create()
        {
            ViewBag.NewBillNo = "BILL-" + DateTime.Now.ToString("yyyyMMdd-HHmm");
            return View(new BillViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BillViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NewBillNo = model.BillNo;
                return View(model);
            }

            if (model.BillItems == null || !model.BillItems.Any())
            {
                ModelState.AddModelError("", "Please add at least one item to the bill.");
                ViewBag.NewBillNo = model.BillNo;
                return View(model);
            }

            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var createdBy = User.FindFirstValue(ClaimTypes.Email) ?? "System";

            var createBillDto = new BillingSystem.Models.DTOs.CreateBillDto
            {
                AgentId = agentId,
                CreatedBy = createdBy,
                CustomerName = model.CustomerName ?? string.Empty,
                CustomerContact = model.CustomerContact,
                Remarks = model.Remarks,
                DiscountPercent = 0,
                TaxPercent = 0,
                Currency = "INR",
                ExchangeRate = 1,
                Items = model.BillItems.Select(i => new BillingSystem.Models.DTOs.BillItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName ?? string.Empty,
                    Quantity = i.Qty,
                    UnitPrice = i.Price,
                    LineTotal = i.Qty * i.Price
                }).ToList()
            };

            var invoiceId = await _mediator.Send(new GenerateBillCommand { BillDto = createBillDto });

            if (invoiceId > 0)
            {
                TempData["Success"] = "Bill created successfully!";
                return RedirectToAction("Invoice", "Agent", new { id = invoiceId });
            }

            ModelState.AddModelError("", "Failed to create bill. Please check server logs.");
            ViewBag.NewBillNo = model.BillNo;
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dashboard = await _mediator.Send(new GetDashboardQuery());
            var billDetails = dashboard.RecentBills.FirstOrDefault(b => b.Id == id);

            if (billDetails == null)
                return NotFound();

            return View(billDetails);
        }

        public async Task<IActionResult> Print(int id)
        {
            var dashboard = await _mediator.Send(new GetDashboardQuery());
            var billDetails = dashboard.RecentBills.FirstOrDefault(b => b.Id == id);

            if (billDetails == null) return NotFound();

            return View("Details", billDetails);
        }
    }
}