using System.Collections.Generic;

namespace StudentFoodParcelApp.Models
{
    public class SrcAdminDashboardViewModel
    {
        public string AdminName { get; set; } = string.Empty;
        public string CampusName { get; set; } = string.Empty;
        public string TermStart { get; set; } = string.Empty;
        public string TermEnd { get; set; } = string.Empty;

        public int TotalApplications { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }

        public int TotalCycles { get; set; }
        public int ActiveCycles { get; set; }
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }

        public List<ApplicationItemViewModel> RecentApplications { get; set; } = new();
        public List<DistributionCollectionItemViewModel> ScheduledDistributions { get; set; } = new();
        public List<ParcelCycleViewModel> RecentCycles { get; set; } = new();
        public List<ApplicationItemViewModel> PendingApplications { get; set; } = new();
        public List<Notification> RecentNotifications { get; set; } = new();
        public int UnreadNotifications { get; set; }
    }
}