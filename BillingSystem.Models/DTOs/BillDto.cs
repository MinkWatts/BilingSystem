namespace BillingSystem.Models.DTOs
{
    // Used to DISPLAY bill
    public class BillDto
    {
        public int Id { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public List<BillItemDto> Items { get; set; }
            = new List<BillItemDto>();
    }

    // One row inside a bill
    public class BillItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    // Used when CREATING a bill
    public class CreateBillDto
    {
        public string AgentId { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public List<BillItemDto> Items { get; set; }
            = new List<BillItemDto>();
    }
}