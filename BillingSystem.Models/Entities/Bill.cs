namespace BillingSystem.Models.Entities
{
    public class Bill : BaseEntity
    {
        public int Id { get; set; }
        public string? AgentId { get; set; }
        public User? Agent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerContact { get; set; }
        public string? Remarks { get; set; }

        // Feature 4: Discount & Tax
        public decimal DiscountPercent { get; set; } = 0;
        public decimal TaxPercent { get; set; } = 18;

        // Feature 3: Multi-currency
        public string Currency { get; set; } = "INR";
        public decimal ExchangeRate { get; set; } = 1;

        public ICollection<BillItem> Items { get; set; }
            = new List<BillItem>();

        public Invoice? Invoice { get; set; }
    }
}