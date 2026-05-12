using BillingSystem.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace BillingSystem.Models.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}