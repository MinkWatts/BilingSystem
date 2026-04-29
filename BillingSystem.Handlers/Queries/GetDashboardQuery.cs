using BillingSystem.Data;
using BillingSystem.Models.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BillingSystem.Handlers.Queries
{
    public class GetDashboardQuery
        : IRequest<DashboardViewModel>
    { }

    public class GetDashboardHandler
        : IRequestHandler<GetDashboardQuery, DashboardViewModel>
    {
        private readonly AppDbContext _db;

        public GetDashboardHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardViewModel> Handle(
            GetDashboardQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var totalSales = await _db.Bills
                    .SumAsync(b => b.TotalAmount,
                        cancellationToken);

                var totalInvoices = await _db.Invoices
                    .CountAsync(cancellationToken);

                return new DashboardViewModel
                {
                    TotalSales = totalSales,
                    TotalInvoices = totalInvoices
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting dashboard");
                return new DashboardViewModel();
            }
        }
    }
}