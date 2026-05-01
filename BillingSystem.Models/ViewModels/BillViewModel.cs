using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Models.ViewModels
{
    public class BillViewModel
    {
        public string BillNo { get; set; }

        [Required]
        [Display(Name = "Bill Date")]
        public DateTime BillDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Customer name is required")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Contact")]
        [MaxLength(15)]
        public string CustomerContact { get; set; }

        public int? CustomerId { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        public List<BillItemDto> BillItems { get; set; }
            = new List<BillItemDto>();

        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public class BillItemDto
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Qty must be at least 1")]
        public int Qty { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public decimal Amount => Qty * Price;
    }
}