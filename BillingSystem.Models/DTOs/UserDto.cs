using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Models.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateUserDto
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
    }

    public class EditUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }
}