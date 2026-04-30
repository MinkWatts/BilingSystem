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

        // Updated: Now fetches real data from the database
        public async Task<IActionResult> Details(int id)
        {
            // This sends a request to your Handlers to get the bill data
            // Note: Ensure you have a 'GetBillDetailsQuery' or similar defined in your Handlers
            var bill = await _mediator.Send(new GetDashboardQuery());

            // Temporary Logic: Since we are using the Dashboard query for now, 
            // we find the specific bill from the RecentBills list.
            var billDetails = bill.RecentBills.FirstOrDefault(b => b.Id == id);

            if (billDetails == null)
            {
                return NotFound();
            }

            return View(billDetails);
        }

        public async Task<IActionResult> Print(int id)
        {
            // Usually, Print uses the same data but a different CSS layout
            var bill = await _mediator.Send(new GetDashboardQuery());
            var billDetails = bill.RecentBills.FirstOrDefault(b => b.Id == id);

            if (billDetails == null) return NotFound();

            return View("Details", billDetails);
        }
    }
}