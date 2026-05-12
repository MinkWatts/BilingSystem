namespace BillingSystem.Models.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public decimal GrandTotal { get; set; }

        // Customer details
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerContact { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

        // Discount & Tax (Feature 4)
        public decimal DiscountPercent { get; set; } = 0;
        public decimal TaxPercent { get; set; } = 18;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }

        // Multi-currency (Feature 3)
        public string Currency { get; set; } = "INR";
        public string CurrencySymbol { get; set; } = "₹";
        public decimal ExchangeRate { get; set; } = 1;

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