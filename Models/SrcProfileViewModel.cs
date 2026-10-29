using System.ComponentModel.DataAnnotations;

namespace StudentFoodParcelApp.Models
{
    public class SrcProfileViewModel
    {
        public int PersonId { get; set; }
        public int SrcAdminId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Emailaddress { get; set; } = string.Empty;

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits (numbers only).")]
        public string? Phonenumber { get; set; }

        public string? Gender { get; set; }
        public string? Dateofbirth { get; set; }
        public string? Username { get; set; }

        public string? CampusName { get; set; }
        public string? UniversityName { get; set; }
        public string? TermStart { get; set; }
        public string? TermEnd { get; set; }

        public string? SuccessMessage { get; set; }
    }
}