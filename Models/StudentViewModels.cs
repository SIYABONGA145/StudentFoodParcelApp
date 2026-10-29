using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentFoodParcelApp.Models
{
    public class StudentProfileViewModel
    {
        public int PersonId { get; set; }
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Student number is required.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Student number must be exactly 9 digits (numbers only).")]
        [Display(Name = "Student Number")]
        public string StudentNumber { get; set; } = string.Empty;

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

        public string CampusName { get; set; } = "N/A";
        public string FundingStatus { get; set; } = "UNVERIFIED";

        public string? SuccessMessage { get; set; }
    }

    public class StudentApplicationViewModel
    {
        public int RequestId { get; set; }
        public int CycleId { get; set; }
        public string CycleName { get; set; } = string.Empty;
        public string RequestDate { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = "PENDING";
        public string? Remarks { get; set; }
        public List<ParcelItemViewModel> CycleItems { get; set; } = new List<ParcelItemViewModel>();

        public string ReferenceNumber
        {
            get
            {
                string year = DateTime.TryParse(RequestDate, out var dt) ? dt.Year.ToString() : DateTime.Now.Year.ToString();
                return $"FPS-{year}-{RequestId:D4}";
            }
        }
    }

    public class CreateApplicationRequestModel
    {
        [Required(ErrorMessage = "Please select a food parcel cycle before continuing.")]
        public int CycleId { get; set; }

        [StringLength(255, ErrorMessage = "The application note is too long. Please shorten it.")]
        public string? Reason { get; set; }
    }

    public class StudentCollectionViewModel
    {
        public int RequestId { get; set; }
        public string CycleName { get; set; } = string.Empty;
        public string CollectionStatus { get; set; } = "PENDING";
        public string DistributionPoint { get; set; } = "Main Campus Pantry";
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;

        public string ReferenceNumber => $"FPS-{DateTime.Now.Year}-{RequestId:D4}";
    }
}