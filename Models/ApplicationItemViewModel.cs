using System.ComponentModel.DataAnnotations;

namespace StudentFoodParcelApp.Models
{
    public class ApplicationItemViewModel
    {
        public int RequestId { get; set; }
        public int StudentId { get; set; }
        public int PersonId { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CampusName { get; set; } = string.Empty;
        public string CycleName { get; set; } = string.Empty;
        public string RequestDate { get; set; } = string.Empty;
        public string FundingStatus { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class UpdateApplicationStatusModel
    {
        [Required(ErrorMessage = "The application could not be found.")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Please select a valid application status.")]
        public string NewStatus { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "The decision reason is too long. Please shorten it and try again.")]
        public string? Remarks { get; set; }
    }

    public class EditStudentProfileDto
    {
        [Required(ErrorMessage = "Invalid student ID.")]
        public int PersonId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Emailaddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits (numbers only).")]
        public string Phonenumber { get; set; } = string.Empty;
    }
}