namespace BillingSystem.Models.DTOs
{
    // Used to DISPLAY bill
    public class BillDto
    {
        public int Id { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

     
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerContact { get; set; }
        public string? Remarks { get; set; }

        public List<BillItemDto> Items { get; set; }
            = new List<BillItemDto>();
    }


    public class BillItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

   
    public class CreateBillDto
    {
        public string AgentId { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;

        
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerContact { get; set; }
        public string? Remarks { get; set; }

        public List<BillItemDto> Items { get; set; }
            = new List<BillItemDto>();
    }
}