namespace BillingSystem.Models.Entities
{
    public class Invoice: BaseEntity
    {
        public int Id { get; set; }

        public int BillId { get; set; }
        public Bill Bill { get; set; }

        // Like INV-0001, INV-0002
        public string InvoiceNumber { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}