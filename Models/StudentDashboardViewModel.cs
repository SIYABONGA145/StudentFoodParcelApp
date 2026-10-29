using System.Collections.Generic;

namespace StudentFoodParcelApp.Models
{
    public class StudentDashboardViewModel
    {
        public string StudentNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string CampusName { get; set; } = string.Empty;

        public int TotalApplications { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int CollectedCount { get; set; }
        public int RejectedCount { get; set; }

        public List<StudentApplicationViewModel> RecentApplications { get; set; } = new();
        public List<ParcelCycleViewModel> ActiveCycles { get; set; } = new();
        public List<Notification> RecentNotifications { get; set; } = new();

        public CollectionViewModel? NextCollection { get; set; } = null;
        public int UnreadNotifications { get; set; }
    }
}