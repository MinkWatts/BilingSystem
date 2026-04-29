namespace BillingSystem.Models.Entities
{
    public class Bill: BaseEntity
    {
        public int Id { get; set; }


        public string AgentId { get; set; }
        public User Agent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        // One bill has many items here....
        public ICollection<BillItem> Items { get; set; }
                = new List<BillItem>();

        // One bill generates one invoice
        public Invoice Invoice { get; set; }
    }
}