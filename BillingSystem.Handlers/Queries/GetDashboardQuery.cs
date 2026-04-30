using BillingSystem.Data;
using BillingSystem.Models.Enums;
using BillingSystem.Models.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BillingSystem.Handlers.Queries
{
    public class GetDashboardQuery : IRequest<DashboardViewModel> { }

    public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, DashboardViewModel>
    {
        private readonly AppDbContext _db;
        
        public GetDashboardHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardViewModel> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Fetch Stats
                var totalSales = await _db.Bills.SumAsync(b => b.TotalAmount, cancellationToken);
                var totalInvoices = await _db.Invoices.CountAsync(cancellationToken);
                var adminCount = await _db.Users.CountAsync(u => u.Role == UserRole.Admin, cancellationToken);
                var agentCount = await _db.Users.CountAsync(u => u.Role == UserRole.Agent, cancellationToken);
                var totalProducts = await _db.Products.CountAsync(cancellationToken);
                var totalCategories = await _db.Categories.CountAsync(cancellationToken);

                // 2. Fetch and Map Recent Bills (Crucial Fix for CS0029)
                var recentBills = await _db.Bills
                    .Include(b => b.Invoice)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .Select(b => new RecentBillViewModel
                    {
                        Id = b.Id,
                        InvoiceNumber = b.Invoice != null ? b.Invoice.InvoiceNumber : "N/A",
                        TotalAmount = b.TotalAmount,
                        CreatedDate = b.CreatedAt,
                        AgentName = b.AgentId ?? "System"
                    })
                    .ToListAsync(cancellationToken);

                // 3. Return the ViewModel
                return new DashboardViewModel
                {
                    TotalSales = totalSales,
                    TotalInvoices = totalInvoices,
                    TotalUsers = adminCount + agentCount,
                    AdminCount = adminCount,
                    AgentCount = agentCount,
                    TotalProducts = totalProducts,
                    TotalCategories = totalCategories,
                    RecentBills = recentBills
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting dashboard data");
                return new DashboardViewModel();
            }
        }
    }
}