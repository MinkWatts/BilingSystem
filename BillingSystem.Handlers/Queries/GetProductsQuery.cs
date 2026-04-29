using BillingSystem.Data;
using BillingSystem.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BillingSystem.Handlers.Queries
{
    public class GetProductsQuery
        : IRequest<List<ProductDto>>
    { }

    public class GetProductsHandler
        : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly AppDbContext _db;

        public GetProductsHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProductDto>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _db.Products
                    .Include(p => p.SubCategory)
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        SubCategoryId = p.SubCategoryId,
                        SubCategoryName = p.SubCategory.Name,
                        IsActive = p.IsActive,
                        CreatedBy = p.CreatedBy,
                        CreatedDate = p.CreatedDate
                    })
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting products");
                return new List<ProductDto>();
            }
        }
    }
}