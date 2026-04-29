namespace BillingSystem.Models.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        // List of subcategories inside this category
        public List<SubCategoryDto> SubCategories { get; set; }
            = new List<SubCategoryDto>();
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class EditCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }
}