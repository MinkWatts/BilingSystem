using Microsoft.AspNetCore.Mvc;
using MediatR;
using BillingSystem.Handlers.Queries;
using Microsoft.AspNetCore.Authorization;
using BillingSystem.Models.ViewModels;

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

         

            TempData["Success"] = "Bill saved successfully!";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int id)
        {
            var bill = await _mediator.Send(new GetDashboardQuery());
            var billDetails = bill.RecentBills.FirstOrDefault(b => b.Id == id);

            if (billDetails == null)
                return NotFound();

            return View(billDetails);
        }

        public async Task<IActionResult> Print(int id)
        {
            var bill = await _mediator.Send(new GetDashboardQuery());
            var billDetails = bill.RecentBills.FirstOrDefault(b => b.Id == id);

            if (billDetails == null) return NotFound();

            return View("Details", billDetails);
        }
    }
}