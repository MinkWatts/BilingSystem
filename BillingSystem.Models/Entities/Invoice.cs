namespace BillingSystem.Models.Entities
{
    public class Invoice: BaseEntity
    {
        public int Id { get; set; }

        public int BillId { get; set; }
        public Bill Bill { get; set; } = null!;

        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}