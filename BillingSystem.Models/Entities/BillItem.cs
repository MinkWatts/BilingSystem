namespace BillingSystem.Models.Entities
{
    public class BillItem: BaseEntity
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public Bill Bill { get; set; }

   
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

  
        public decimal LineTotal => Quantity * UnitPrice;
    }
}