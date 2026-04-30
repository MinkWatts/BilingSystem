using BillingSystem.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _mediator
                .Send(new GetDashboardQuery());
            return View(data);
        }

        public async Task<IActionResult> Reports()
        {
            var data = await _mediator
                .Send(new GetDashboardQuery());
            return View(data);
        }
    }
}