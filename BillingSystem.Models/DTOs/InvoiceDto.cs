namespace BillingSystem.Models.DTOs
{
    // Used to DISPLAY invoice
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public decimal GrandTotal { get; set; }

        // List of items inside invoice
        public List<InvoiceItemDto> Items { get; set; }
            = new List<InvoiceItemDto>();
    }

    // One row inside invoice table
    public class InvoiceItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}