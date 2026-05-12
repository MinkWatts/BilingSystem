using BillingSystem.Data;
using BillingSystem.Models.DTOs;
using BillingSystem.Models.Entities;
using MediatR;
using Serilog;

namespace BillingSystem.Handlers.Commands
{
    public class AddSubCategoryCommand : IRequest<bool>
    {
        public CreateSubCategoryDto SubCategoryDto { get; set; }
            = null!;
    }

    public class AddSubCategoryHandler
        : IRequestHandler<AddSubCategoryCommand, bool>
    {
        private readonly AppDbContext _db;

        public AddSubCategoryHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Handle(
            AddSubCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var subCategory = new SubCategory
                {
                   Name = request.SubCategoryDto.Name,
                    CategoryId = request.SubCategoryDto.CategoryId,
                    CreatedBy = request.SubCategoryDto.CreatedBy,
                    CreatedDate = DateTime.Now
                };

                _db.SubCategories.Add(subCategory);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding subcategory");
                return false;
            }
        }
    }
}