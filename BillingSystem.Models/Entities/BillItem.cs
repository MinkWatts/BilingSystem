namespace BillingSystem.Models.Entities
{
    public class BillItem: BaseEntity
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public Bill Bill { get; set; }

        // Which product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        // Price stored at time of bill
        public decimal UnitPrice { get; set; }

  
        public decimal LineTotal => Quantity * UnitPrice;
    }
}