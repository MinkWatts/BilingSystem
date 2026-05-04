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

        public ICollection<BillItem> Items { get; set; }
            = new List<BillItem>();

        public Invoice? Invoice { get; set; }
    }
}