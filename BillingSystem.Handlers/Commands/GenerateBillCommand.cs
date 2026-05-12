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
                if (request.BillDto == null)
                    throw new ArgumentNullException(nameof(request.BillDto), "BillDto cannot be null.");

                if (request.BillDto.Items == null || !request.BillDto.Items.Any())
                    throw new ArgumentException("Bill must contain at least one item.");

                var billItems = new List<BillItem>();

                foreach (var item in request.BillDto.Items)
                {
                    var product = await _db.Products
                        .FindAsync(new object[] { item.ProductId }, cancellationToken);

                    if (product == null)
                    {
                        Log.Warning("Product with ID {ProductId} not found. Skipping.", item.ProductId);
                        continue;
                    }

                    billItems.Add(new BillItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        CreatedBy = request.BillDto.CreatedBy,
                        CreatedDate = DateTime.Now
                    });
                }

                if (!billItems.Any())
                    throw new InvalidOperationException("No valid products found for the bill items provided.");

                // Calculate subtotal before discount/tax
                var subTotal = billItems.Sum(x => x.UnitPrice * x.Quantity);
                var discountAmount = subTotal * (request.BillDto.DiscountPercent / 100m);
                var afterDiscount = subTotal - discountAmount;
                var taxAmount = afterDiscount * (request.BillDto.TaxPercent / 100m);
                var grandTotal = afterDiscount + taxAmount;

                var bill = new Bill
                {
                    AgentId = request.BillDto.AgentId,
                    TotalAmount = grandTotal,
                    Items = billItems,
                    CreatedBy = request.BillDto.CreatedBy,
                    CreatedDate = DateTime.Now,

                    // Customer details
                    CustomerName = request.BillDto.CustomerName,
                    CustomerContact = request.BillDto.CustomerContact,
                    Remarks = request.BillDto.Remarks,

                    // Discount & Tax
                    DiscountPercent = request.BillDto.DiscountPercent,
                    TaxPercent = request.BillDto.TaxPercent,

                    // Multi-currency
                    Currency = request.BillDto.Currency,
                    ExchangeRate = request.BillDto.ExchangeRate
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

                Log.Information("Bill #{BillId} created successfully. Invoice #{InvoiceId} generated.", bill.Id, invoice.Id);
                return invoice.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating bill for agent {AgentId}", request.BillDto?.AgentId);
                return 0;
            }
        }
    }
}