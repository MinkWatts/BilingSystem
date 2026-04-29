using BillingSystem.Data;
using BillingSystem.Models.DTOs;
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

                return new InvoiceDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    AgentName = invoice.Bill.Agent.FullName,
                    GeneratedAt = invoice.GeneratedAt,
                    GrandTotal = invoice.Bill.TotalAmount,
                    Items = invoice.Bill.Items
                        .Select(i => new InvoiceItemDto
                        {
                            ProductName = i.Product.Name,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            LineTotal = i.Quantity * i.UnitPrice
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting invoice");
                return new InvoiceDto();
            }
        }
    }
}