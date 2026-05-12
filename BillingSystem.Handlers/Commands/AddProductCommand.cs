using BillingSystem.Data;
using BillingSystem.Models.DTOs;
using BillingSystem.Models.Entities;
using MediatR;
using Serilog;

namespace BillingSystem.Handlers.Commands
{
    public class AddProductCommand : IRequest<bool>
    {
        public CreateProductDto ProductDto { get; set; } = null!;
    }

    public class AddProductHandler
        : IRequestHandler<AddProductCommand, bool>
    {
        private readonly AppDbContext _db;

        public AddProductHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Handle(
            AddProductCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var product = new Product
                {
                    Name = request.ProductDto.Name,
                    Price = request.ProductDto.Price,
                    SubCategoryId = request.ProductDto.SubCategoryId,
                    IsActive = true,
                    CreatedBy = request.ProductDto.CreatedBy,
                    CreatedDate = DateTime.Now
                };

                _db.Products.Add(product);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding product");
                return false;
            }
        }
    }
}