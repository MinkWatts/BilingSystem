namespace BillingSystem.Models.ViewModels
{
    public class InvoiceViewModel
    {
        public string InvoiceNumber { get; set; }
        public string AgentName { get; set; }
        public DateTime GeneratedAt { get; set; }
        public decimal GrandTotal { get; set; }

        public List<InvoiceItemViewModel> Items { get; set; }
                = new List<InvoiceItemViewModel>();
    }

    public class InvoiceItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}