using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentFoodParcelApp.Models
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Please select an account role.")]
        public string Role { get; set; } = "Student";

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150, ErrorMessage = "Email address cannot exceed 150 characters.")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits (numbers only).")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a gender.")]
        public string Gender { get; set; } = "Male";

        [Required(ErrorMessage = "Date of birth is required.")]
        [Display(Name = "Date of Birth")]
        public string DateOfBirth { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a university.")]
        [Display(Name = "University")]
        public int UniversityId { get; set; }

        [Required(ErrorMessage = "Please select a campus.")]
        [Display(Name = "Campus")]
        public int CampusId { get; set; }

        [RegularExpression(@"^\d{9}$", ErrorMessage = "Student number must be exactly 9 digits (numbers only).")]
        [Display(Name = "Student Number")]
        public string? StudentNumber { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Term Start Date")]
        public string? TermStart { get; set; }

        [Display(Name = "Term End Date")]
        public string? TermEnd { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == "Student" && string.IsNullOrWhiteSpace(StudentNumber))
            {
                yield return new ValidationResult(
                    "Student number is required for students.",
                    new[] { nameof(StudentNumber) }
                );
            }
        }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}