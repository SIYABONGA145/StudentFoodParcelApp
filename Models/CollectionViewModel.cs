using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models
{
    public class CollectionViewModel
    {
        public int RequestId { get; set; }
        public string CycleName { get; set; } = string.Empty;
        public string DistributionDate { get; set; } = string.Empty;
        public string DistributionTime { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string CampusName { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public string VerificationToken { get; set; } = string.Empty;
    }

    public class DistributionCollectionItemViewModel
    {
        public int RequestId { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string CampusName { get; set; } = string.Empty;
        public string CycleName { get; set; } = string.Empty;
        public string DistributionDate { get; set; } = string.Empty;
        public string DistributionTime { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string ApplicationStatus { get; set; } = "APPROVED";
        public string CollectionStatus { get; set; } = "PENDING";
        public string? CollectionDate { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
        public string ScheduledDate { get; set; } = string.Empty;
    }

    public class MarkCollectionDto
    {
        public int RequestId { get; set; }
        public string CollectionStatus { get; set; } = string.Empty;
    }

    public class ScheduleDistributionDto
    {
        public int RequestId { get; set; }
        public string DistributionDate { get; set; } = string.Empty;
        public string DistributionTime { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
    }
}