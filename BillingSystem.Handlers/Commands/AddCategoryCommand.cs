using BillingSystem.Data;
using BillingSystem.Models.DTOs;
using BillingSystem.Models.Entities;
using MediatR;
using Serilog;

namespace BillingSystem.Handlers.Commands
{
    public class AddCategoryCommand : IRequest<bool>
    {
        public CreateCategoryDto CategoryDto { get; set; } = null!;
    }

    public class AddCategoryHandler
        : IRequestHandler<AddCategoryCommand, bool>
    {
        private readonly AppDbContext _db;

        public AddCategoryHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Handle(
            AddCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var category = new Category
                {
                    Name = request.CategoryDto.Name,
                    CreatedBy = request.CategoryDto.CreatedBy,
                    CreatedDate = DateTime.Now
                };

                _db.Categories.Add(category);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding category");
                return false;
            }
        }
    }
}