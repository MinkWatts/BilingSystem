using BillingSystem.Data;
using BillingSystem.Models.DTOs;
using BillingSystem.Models.Entities;
using MediatR;
using Serilog;

namespace BillingSystem.Handlers.Commands
{
    public class GenerateBillCommand : IRequest<int>
    {
        public CreateBillDto BillDto { get; set; } = null!;
    }

    public class GenerateBillHandler
        : IRequestHandler<GenerateBillCommand, int>
    {
        private readonly AppDbContext _db;

        public GenerateBillHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> Handle(
            GenerateBillCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var billItems = new List<BillItem>();

                foreach (var item in request.BillDto.Items)
                {
                    var product = await _db.Products
                        .FindAsync(item.ProductId);

                    if (product == null) continue;

                    billItems.Add(new BillItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        CreatedBy = request.BillDto.CreatedBy,
                        CreatedDate = DateTime.Now
                    });
                }

                var bill = new Bill
                {
                    AgentId = request.BillDto.AgentId,
                    TotalAmount = billItems.Sum(x => x.UnitPrice * x.Quantity),
                    Items = billItems,
                    CreatedBy = request.BillDto.CreatedBy,
                    CreatedDate = DateTime.Now,

                    // ── Customer details ──
                    CustomerName = request.BillDto.CustomerName,
                    CustomerContact = request.BillDto.CustomerContact,
                    Remarks = request.BillDto.Remarks
                };

                _db.Bills.Add(bill);
                await _db.SaveChangesAsync(cancellationToken);

                var invoice = new Invoice
                {
                    BillId = bill.Id,
                    InvoiceNumber = $"INV-{bill.Id:D4}",
                    GeneratedAt = DateTime.Now,
                    CreatedBy = request.BillDto.CreatedBy,
                    CreatedDate = DateTime.Now
                };

                _db.Invoices.Add(invoice);
                await _db.SaveChangesAsync(cancellationToken);

                return invoice.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating bill");
                return 0;
            }
        }
    }
}