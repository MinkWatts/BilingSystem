namespace BillingSystem.Models.Entities
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
            = string.Empty;
        public decimal Price { get; set; }
        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
            = null!;
        public bool IsActive { get; set; } = true;
    }
}