using BillingSystem.Data;
using BillingSystem.Models.DTOs;
using BillingSystem.Models.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BillingSystem.Handlers.Queries
{
    public class GetInvoiceQuery : IRequest<InvoiceDto>
    {
        public int InvoiceId { get; set; }
    }

    public class GetInvoiceHandler
        : IRequestHandler<GetInvoiceQuery, InvoiceDto>
    {
        private readonly AppDbContext _db;

        public GetInvoiceHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<InvoiceDto> Handle(
            GetInvoiceQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var invoice = await _db.Invoices
                    .Include(i => i.Bill)
                        .ThenInclude(b => b.Items)
                            .ThenInclude(bi => bi.Product)
                    .Include(i => i.Bill.Agent)
                    .FirstOrDefaultAsync(
                        i => i.Id == request.InvoiceId,
                        cancellationToken);

                if (invoice == null) return new InvoiceDto();

                var bill = invoice.Bill;

                // Calculate totals correctly
                var subTotal = bill.Items.Sum(i => i.Quantity * i.UnitPrice);
                var discountAmount = subTotal * (bill.DiscountPercent / 100m);
                var afterDiscount = subTotal - discountAmount;
                var taxAmount = afterDiscount * (bill.TaxPercent / 100m);
                var grandTotal = afterDiscount + taxAmount;

                return new InvoiceDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    AgentName = bill.Agent?.FullName ?? "Agent",
                    GeneratedAt = invoice.GeneratedAt,

                    CustomerName = bill.CustomerName,
                    CustomerContact = bill.CustomerContact ?? string.Empty,
                    Remarks = bill.Remarks ?? string.Empty,

                    // Discount & Tax
                    DiscountPercent = bill.DiscountPercent,
                    TaxPercent = bill.TaxPercent,
                    SubTotal = subTotal,
                    DiscountAmount = discountAmount,
                    TaxAmount = taxAmount,
                    GrandTotal = grandTotal,

                    // Multi-currency
                    Currency = bill.Currency,
                    ExchangeRate = bill.ExchangeRate,
                    CurrencySymbol = CurrencyHelper.GetSymbol(bill.Currency),

                    Items = bill.Items
                        .Select(i => new InvoiceItemDto
                        {
                            ProductName = i.Product?.Name ?? "Unknown Product",
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            LineTotal = i.Quantity * i.UnitPrice
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting invoice {InvoiceId}", request.InvoiceId);
                return new InvoiceDto();
            }
        }
    }
}