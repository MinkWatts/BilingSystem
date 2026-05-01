namespace BillingSystem.Models.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public decimal GrandTotal { get; set; }

   
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerContact { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

        public List<InvoiceItemDto> Items { get; set; }
            = new List<InvoiceItemDto>();
    }

    public class InvoiceItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}