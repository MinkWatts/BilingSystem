namespace BillingSystem.Models.ViewModels
{
    public class BillViewModel
    {
        public List<BillItemDto> Items { get; set; }
                = new List<BillItemDto>();
    }

    public class BillItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}