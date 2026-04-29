namespace BillingSystem.Models.Entities
{
    public class Category: BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }
                = new List<SubCategory>();

        public ICollection<Product> Products { get; set; }
                = new List<Product>();
    }
}