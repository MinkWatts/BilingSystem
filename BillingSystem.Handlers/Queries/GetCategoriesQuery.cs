using BillingSystem.Data;
using BillingSystem.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BillingSystem.Handlers.Queries
{
    public class GetCategoriesQuery
        : IRequest<List<CategoryDto>>
    { }

    public class GetCategoriesHandler
        : IRequestHandler<GetCategoriesQuery,
            List<CategoryDto>>
    {
        private readonly AppDbContext _db;

        public GetCategoriesHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryDto>> Handle(
            GetCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _db.Categories
                    .Include(c => c.SubCategories)
                    .ToListAsync(cancellationToken);

                var result = new List<CategoryDto>();

                foreach (var c in categories)
                {
                    var subList = new List<SubCategoryDto>();

                    foreach (var s in c.SubCategories)
                    {
                        subList.Add(new SubCategoryDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            CategoryId = s.CategoryId,
                            CategoryName = c.Name,
                            CreatedBy = s.CreatedBy,
                            CreatedDate = s.CreatedDate
                        });
                    }

                    result.Add(new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        CreatedBy = c.CreatedBy,
                        CreatedDate = c.CreatedDate,
                        SubCategories = subList
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting categories");
                return new List<CategoryDto>();
            }
        }
    }
}