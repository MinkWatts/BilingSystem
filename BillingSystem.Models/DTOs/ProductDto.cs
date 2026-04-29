namespace BillingSystem.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SubCategoryId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class EditProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SubCategoryId { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}