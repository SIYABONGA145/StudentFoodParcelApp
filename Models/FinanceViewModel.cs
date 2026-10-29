using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentFoodParcelApp.Models
{
    public class StudentFundingVerificationViewModel
    {
        public int StudentId { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CampusName { get; set; } = string.Empty;
        public bool IsFunded { get; set; }
        public bool IsVerified { get; set; }

        [StringLength(255, ErrorMessage = "Reason cannot exceed 255 characters.")]
        public string Reason { get; set; } = string.Empty;
    }

    public class StudentFundingUpdateModel
    {
        [Required(ErrorMessage = "Student ID is required.")]
        public int StudentId { get; set; }

        public bool IsFunded { get; set; }

        [StringLength(255, ErrorMessage = "Reason cannot exceed 255 characters.")]
        public string Reason { get; set; } = string.Empty;
    }

    public class FinanceStudentsViewModel
    {
        public int? SelectedCycleId { get; set; }
        public List<SelectListItem> AvailableCycles { get; set; } = new List<SelectListItem>();
        public List<StudentFundingVerificationViewModel> Students { get; set; } = new List<StudentFundingVerificationViewModel>();
    }
}