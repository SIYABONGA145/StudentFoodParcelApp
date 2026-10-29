using System.Linq;
using StudentFoodParcelApp.Models;

namespace StudentFoodParcelApp.Models
{
    public static class RequestExtensions
    {
        public static string LatestStatus(this Request r)
        {
            if (r?.ApplicationStatuses == null || !r.ApplicationStatuses.Any())
                return "PENDING";

            return r.ApplicationStatuses
                .OrderByDescending(a => a.Applicationstatusid)
                .FirstOrDefault()?.Applicationstatus ?? "PENDING";
        }

        public static ApplicationStatus? LatestStatusRecord(this Request r)
        {
            if (r?.ApplicationStatuses == null || !r.ApplicationStatuses.Any())
                return null;

            return r.ApplicationStatuses
                .OrderByDescending(a => a.Applicationstatusid)
                .FirstOrDefault();
        }

        public static bool IsPendingCollection(this Request r)
        {
            var s = r.LatestStatus();
            return s.Equals("APPROVED", System.StringComparison.OrdinalIgnoreCase)
                || s.Equals("READY_FOR_COLLECTION", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}