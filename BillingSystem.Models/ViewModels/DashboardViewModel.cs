using System;
using System.Collections.Generic;

namespace BillingSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalSales { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalUsers { get; set; }
        public int AdminCount { get; set; }  // Fixed name
        public int AgentCount { get; set; }  // Fixed name
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public List<RecentBillViewModel> RecentBills { get; set; } = new();
    }

    public class RecentBillViewModel
    {
        public int Id { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; } // Fixed name
        public string InvoiceNumber { get; set; } = string.Empty;
    }
}