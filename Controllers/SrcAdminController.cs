#nullable enable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentFoodParcelApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentFoodParcelApp.Controllers
{
    [Authorize(Roles = "SRCAdmin")]
    public class SrcAdminController : Controller
    {
        private readonly StudentFoodParcelDbContext _context;

        public SrcAdminController(StudentFoodParcelDbContext context)
        {
            _context = context;
        }

        private async Task<(Srcadmin? admin, int campusId, string campusName)> GetCurrentAdminCampusAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return (null, 0, "Main Campus");

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Username == username);
            if (person == null) return (null, 0, "Main Campus");

            var srcAdmin = await _context.Srcadmins
                .Include(s => s.Campus)
                .FirstOrDefaultAsync(s => s.Personid == person.Personid);

            int campusId = srcAdmin?.Campusid ?? 0;
            string campusName = srcAdmin?.Campus?.Campusname ?? "Main Campus";

            return (srcAdmin, campusId, campusName);
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction("Dashboard");

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var (admin, campusId, campusName) = await GetCurrentAdminCampusAsync();

            var requestsQuery = _context.Requests
                .Where(r => r.Student != null && (campusId == 0 || r.Student.Campusid == campusId));

            var totalApplications = await requestsQuery.CountAsync();
            var pendingCount = await requestsQuery.CountAsync(r =>
                r.ApplicationStatuses.OrderByDescending(s => s.Applicationstatusid).FirstOrDefault()!.Applicationstatus == "PENDING");
            var approvedCount = await requestsQuery.CountAsync(r =>
                r.ApplicationStatuses.OrderByDescending(s => s.Applicationstatusid).FirstOrDefault()!.Applicationstatus == "APPROVED");
            var rejectedCount = await requestsQuery.CountAsync(r =>
                r.ApplicationStatuses.OrderByDescending(s => s.Applicationstatusid).FirstOrDefault()!.Applicationstatus == "REJECTED");

            var rawRequests = await _context.Requests
                .Include(r => r.Student).ThenInclude(s => s!.Person)
                .Include(r => r.Student).ThenInclude(s => s!.Campus)
                .Include(r => r.Cycle)
                .Include(r => r.ApplicationStatuses)
                .Where(r => r.Student != null && (campusId == 0 || r.Student.Campusid == campusId))
                .OrderByDescending(r => r.Requestid)
                .Take(5)
                .ToListAsync();

            var recentApplications = rawRequests.Select(r => new ApplicationItemViewModel
            {
                RequestId = r.Requestid,
                StudentId = r.Studentid ?? 0,
                PersonId = r.Student?.Person?.Personid ?? 0,
                StudentNumber = r.Student?.Studentnumber ?? "N/A",
                StudentName = r.Student?.Person != null
                    ? $"{r.Student.Person.Firstname ?? ""} {r.Student.Person.Lastname ?? ""}".Trim()
                    : "N/A",
                Email = r.Student?.Person?.Emailaddress ?? "",
                PhoneNumber = r.Student?.Person?.Phonenumber ?? "",
                CampusName = r.Student?.Campus?.Campusname ?? "Main Campus",
                CycleName = r.Cycle?.Cyclename ?? "N/A",
                RequestDate = r.Requestdate ?? "",
                CurrentStatus = r.ApplicationStatuses
                    .OrderByDescending(s => s.Applicationstatusid)
                    .Select(s => s.Applicationstatus)
                    .FirstOrDefault() ?? "PENDING",
                Remarks = r.ApplicationStatuses
                    .OrderByDescending(s => s.Applicationstatusid)
                    .Select(s => s.DecisionReason)
                    .FirstOrDefault() ?? ""
            }).ToList();

            var rawScheduled = await _context.Distributionrequests
                .Include(dr => dr.Request).ThenInclude(r => r!.Student).ThenInclude(s => s!.Person)
                .Include(dr => dr.Request).ThenInclude(r => r!.ApplicationStatuses)
                .Include(dr => dr.Distribution).ThenInclude(d => d!.Venue)
                .Where(dr => dr.Request != null && dr.Request.Student != null
                          && (campusId == 0 || dr.Request.Student.Campusid == campusId))
                .OrderByDescending(dr => dr.Distribution != null ? dr.Distribution.Distributiondate : null)
                .Take(5)
                .ToListAsync();

            var scheduledDistributions = rawScheduled.Select(dr =>
            {
                var req = dr.Request!;
                var dist = dr.Distribution;

                string currentStatus = req.ApplicationStatuses
                    .OrderByDescending(s => s.Applicationstatusid)
                    .Select(s => s.Applicationstatus)
                    .FirstOrDefault() ?? "READY_FOR_COLLECTION";

                return new DistributionCollectionItemViewModel
                {
                    RequestId = req.Requestid,
                    StudentNumber = req.Student?.Studentnumber ?? "N/A",
                    StudentName = req.Student?.Person != null
                        ? $"{req.Student.Person.Firstname} {req.Student.Person.Lastname}".Trim()
                        : "N/A",
                    CycleName = req.Cycle?.Cyclename ?? "",
                    DistributionDate = dist?.Distributiondate?.ToString("yyyy-MM-dd") ?? "N/A",
                    DistributionTime = dist?.Distributiontime ?? "N/A",
                    VenueName = dist?.Venue?.Venuename ?? "N/A",
                    CollectionStatus = currentStatus == "COLLECTED" || currentStatus == "NOT COLLECTED" ? currentStatus : "SCHEDULED",
                    CurrentStatus = currentStatus,
                    ScheduledDate = (dist?.Distributiondate != null
                        ? $"{dist.Distributiondate:yyyy-MM-dd} {dist.Distributiontime}"
                        : "TBC")
                };
            }).ToList();

            var recentNotifications = await _context.Notifications
                .Where(n => admin == null || n.Srcadminid == admin.Srcadminid || n.Srcadminid == null)
                .OrderByDescending(n => n.Createddate)
                .Take(5)
                .ToListAsync();

            var unreadCount = recentNotifications.Count(n =>
                !string.Equals(n.Isread, "YES", StringComparison.OrdinalIgnoreCase));

            var person = admin?.Personid != null
                ? await _context.Persons.FirstOrDefaultAsync(p => p.Personid == admin.Personid)
                : null;

            var vm = new SrcAdminDashboardViewModel
            {
                AdminName = person != null
                    ? $"{person.Firstname} {person.Lastname}".Trim()
                    : "SRC Admin",
                CampusName = campusName,
                TermStart = admin?.Termstart ?? "",
                TermEnd = admin?.Termend ?? "",

                TotalApplications = totalApplications,
                PendingCount = pendingCount,
                ApprovedCount = approvedCount,
                RejectedCount = rejectedCount,

                TotalRequests = totalApplications,
                PendingRequests = pendingCount,
                ApprovedRequests = approvedCount,
                RejectedRequests = rejectedCount,

                RecentApplications = recentApplications,
                ScheduledDistributions = scheduledDistributions,
                RecentNotifications = recentNotifications,
                UnreadNotifications = unreadCount
            };

            ViewBag.TotalApplications = totalApplications;
            ViewBag.PendingCount = pendingCount;
            ViewBag.ApprovedCount = approvedCount;
            ViewBag.RejectedCount = rejectedCount;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Applications()
        {
            var (_, campusId, _) = await GetCurrentAdminCampusAsync();

            var rawRequests = await _context.Requests
                .Include(r => r.Student).ThenInclude(s => s!.Person)
                .Include(r => r.Student).ThenInclude(s => s!.Campus)
                .Include(r => r.Cycle)
                .Include(r => r.ApplicationStatuses)
                .Where(r => r.Student != null && (campusId == 0 || r.Student.Campusid == campusId))
                .OrderByDescending(r => r.Requestid)
                .ToListAsync();

            var fundingStatuses = await _context.Fundingstatuses.ToListAsync();

            var applications = rawRequests.Select(r =>
            {
                var studentPerson = r.Student?.Person;
                var studentCampus = r.Student?.Campus;

                var studentFunding = fundingStatuses
                    .Where(f => f.Studentid == r.Studentid)
                    .OrderByDescending(f => f.Fundingstatusid)
                    .FirstOrDefault();

                string fundingVal = studentFunding?.Fundingstatus1 ?? studentFunding?.Reason ?? "UNFUNDED";

                return new ApplicationItemViewModel
                {
                    RequestId = r.Requestid,
                    StudentId = r.Studentid ?? 0,
                    PersonId = studentPerson?.Personid ?? 0,
                    StudentNumber = r.Student?.Studentnumber ?? "N/A",
                    StudentName = studentPerson != null ? $"{studentPerson.Firstname ?? ""} {studentPerson.Lastname ?? ""}".Trim() : "N/A",
                    Email = studentPerson?.Emailaddress ?? "",
                    PhoneNumber = studentPerson?.Phonenumber ?? "",
                    CampusName = studentCampus?.Campusname ?? "Main Campus",
                    CycleName = r.Cycle?.Cyclename ?? "N/A",
                    RequestDate = r.Requestdate ?? "",
                    FundingStatus = fundingVal,
                    CurrentStatus = r.ApplicationStatuses
                        .OrderByDescending(s => s.Applicationstatusid)
                        .Select(s => s.Applicationstatus)
                        .FirstOrDefault() ?? "PENDING",
                    Remarks = r.ApplicationStatuses
                        .OrderByDescending(s => s.Applicationstatusid)
                        .Select(s => s.DecisionReason)
                        .FirstOrDefault() ?? ""
                };
            }).ToList();

            return View(applications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateApplicationStatusModel? dto)
        {
            if (dto == null || dto.RequestId <= 0 || string.IsNullOrEmpty(dto.NewStatus))
                return Json(new { success = false, message = "Invalid status update payload." });

            var (admin, campusId, _) = await GetCurrentAdminCampusAsync();
            if (admin == null) return Json(new { success = false, message = "Unauthorized SRC administrator." });

            var request = await _context.Requests
                .Include(r => r.Student)
                .Where(r => r.Student != null && r.Student.Campusid == campusId)
                .FirstOrDefaultAsync(r => r.Requestid == dto.RequestId);

            if (request == null)
                return Json(new { success = false, message = "Application request not found or outside your campus jurisdiction." });

            string statusUpper = dto.NewStatus.ToUpper();
            string refNumber = $"FPS-{request.Requestid:D4}";

            var status = new ApplicationStatus
            {
                Requestid = dto.RequestId,
                Applicationstatus = statusUpper,
                DecisionReason = string.IsNullOrWhiteSpace(dto.Remarks) ? $"Status updated to {statusUpper}" : dto.Remarks,
                DecisionDate = DateTime.Now,
                Srcadminid = admin.Srcadminid
            };

            _context.ApplicationStatuses.Add(status);

            if (request.Studentid.HasValue)
            {
                _context.Notifications.Add(new Notification
                {
                    Studentid = request.Studentid,
                    Notificationtype = "APPLICATION_DECISION",
                    Title = "Application Status Update",
                    Message = $"Your food parcel application {refNumber} has been {statusUpper}.",
                    Isread = "NO",
                    Createddate = DateTime.Now
                });

                _context.Notifications.Add(new Notification
                {
                    Srcadminid = admin.Srcadminid,
                    Notificationtype = "APPLICATION_DECISION_ADMIN",
                    Title = $"Application {refNumber} {statusUpper}",
                    Message = $"Application {refNumber} for {request.Student?.Person?.Firstname} {request.Student?.Person?.Lastname} was updated to {statusUpper}.",
                    Isread = "NO",
                    Createddate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Status updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudentProfile([FromBody] EditStudentProfileDto? dto)
        {
            if (dto == null || dto.PersonId <= 0)
                return Json(new { success = false, message = "Invalid student profile data." });

            var person = await _context.Persons.FindAsync(dto.PersonId);
            if (person == null)
                return Json(new { success = false, message = "Student person record not found." });

            person.Firstname = dto.Firstname.Trim();
            person.Lastname = dto.Lastname.Trim();
            person.Emailaddress = dto.Emailaddress.Trim();
            person.Phonenumber = dto.Phonenumber.Trim();

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Student details updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> Cycles()
        {
            var cyclesData = await _context.Foodparcelcycles
                .Include(c => c.Cycleitems)
                .OrderByDescending(c => c.Cycleid)
                .ToListAsync();

            var cycles = cyclesData.Select(c => new ParcelCycleViewModel
            {
                CycleId = c.Cycleid,
                CycleName = c.Cyclename ?? "",
                StartDate = c.Startdate?.ToString("yyyy-MM-dd") ?? "",
                EndDate = c.Enddate?.ToString("yyyy-MM-dd") ?? "",
                Items = c.Cycleitems.Select(i => new ParcelItemViewModel
                {
                    ItemId = i.Cycleitemid,
                    ItemName = i.Itemname ?? "",
                    Quantity = int.TryParse(i.Quantity, out var q) ? q : 1
                }).ToList()
            }).ToList();

            return View(cycles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBulkDistribution([FromBody] BulkScheduleDistributionDto? dto)
        {
            // 1. Basic Validation
            if (dto == null || dto.RequestIds == null || !dto.RequestIds.Any() ||
                string.IsNullOrWhiteSpace(dto.VenueName) || string.IsNullOrWhiteSpace(dto.DistributionDate) || string.IsNullOrWhiteSpace(dto.DistributionTime))
                return Json(new { success = false, message = "Please select students and complete all scheduling fields." });

            if (!DateTime.TryParse(dto.DistributionDate, out var distDate) || distDate.Date < DateTime.Today)
                return Json(new { success = false, message = "The distribution date cannot be earlier than today." });

            var (admin, campusId, _) = await GetCurrentAdminCampusAsync();
            if (admin == null) return Json(new { success = false, message = "Unauthorized SRC administrator." });

            // 2. Fetch all valid requests in one go (Much faster than looping DB queries)
            var requests = await _context.Requests
                .Include(r => r.Student)
                .Include(r => r.Cycle)
                .Include(r => r.ApplicationStatuses)
                .Where(r => r.Student != null && r.Student.Campusid == campusId && dto.RequestIds.Contains(r.Requestid))
                .ToListAsync();

            var validRequests = new List<Request>();
            foreach (var req in requests)
            {
                string latestStatus = req.ApplicationStatuses
                    .OrderByDescending(s => s.Applicationstatusid)
                    .Select(s => s.Applicationstatus)
                    .FirstOrDefault() ?? "";

                if (latestStatus == "APPROVED")
                {
                    validRequests.Add(req);
                }
            }

            if (!validRequests.Any())
                return Json(new { success = false, message = "None of the selected applications are in an APPROVED state or they are outside your campus." });

            // 3. Process everything inside a Database Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Find or create the Venue once for all students
                var venue = await _context.Venues.FirstOrDefaultAsync(v => v.Venuename == dto.VenueName && v.Campusid == campusId);
                if (venue == null)
                {
                    venue = new Venue { Venuename = dto.VenueName, Campusid = campusId };
                    _context.Venues.Add(venue);
                    await _context.SaveChangesAsync();
                }

                // Prepare lists for bulk insert (Fast performance)
                var distributionsToAdd = new List<Distribution>();
                var statusesToAdd = new List<ApplicationStatus>();
                var notificationsToAdd = new List<Notification>();

                foreach (var request in validRequests)
                {
                    // Create Distribution
                    var distribution = new Distribution
                    {
                        Distributiondate = DateOnly.FromDateTime(distDate),
                        Distributiontime = dto.DistributionTime,
                        Cycleid = request.Cycleid,
                        Venueid = venue.Venueid,
                        Srcadminid = admin.Srcadminid
                    };
                    distributionsToAdd.Add(distribution);

                    // Create Status
                    statusesToAdd.Add(new ApplicationStatus
                    {
                        Requestid = request.Requestid,
                        Applicationstatus = "READY_FOR_COLLECTION",
                        DecisionReason = $"Scheduled for {dto.DistributionDate} at {dto.DistributionTime} at {venue.Venuename}",
                        DecisionDate = DateTime.Now,
                        Srcadminid = admin.Srcadminid
                    });

                    // Create Notification
                    string refNumber = $"FPS-{request.Requestid:D4}";
                    notificationsToAdd.Add(new Notification
                    {
                        Studentid = request.Studentid,
                        Notificationtype = "COLLECTION_READY",
                        Title = "Parcel Ready for Collection",
                        Message = $"Your food parcel ({refNumber}) is ready for collection on {dto.DistributionDate} at {dto.DistributionTime} at {venue.Venuename}.",
                        Isread = "NO",
                        Createddate = DateTime.Now,
                        Srcadminid = admin.Srcadminid
                    });
                }

                // Add Distributions first so they get IDs
                _context.Distributions.AddRange(distributionsToAdd);
                await _context.SaveChangesAsync(); // Save to generate Distribution IDs

                // Now link the Distribution IDs to the Requests
                var distRequestsToAdd = new List<Distributionrequest>();
                for (int i = 0; i < validRequests.Count; i++)
                {
                    distRequestsToAdd.Add(new Distributionrequest
                    {
                        Distributionid = distributionsToAdd[i].Distributionid,
                        Requestid = validRequests[i].Requestid
                    });

                    // Link notification to the generated Distribution ID
                    notificationsToAdd[i].Distributionid = distributionsToAdd[i].Distributionid;
                }

                _context.Distributionrequests.AddRange(distRequestsToAdd);
                _context.ApplicationStatuses.AddRange(statusesToAdd);
                _context.Notifications.AddRange(notificationsToAdd);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = $"Successfully scheduled {validRequests.Count} students for collection." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "A database error occurred. No schedules were saved. Error: " + ex.Message });
            }
        }

        // Add this DTO class at the very bottom of your Controller, near your other DTOs
        public class BulkScheduleDistributionDto
        {
            public List<int> RequestIds { get; set; } = new();
            public string? DistributionDate { get; set; }
            public string? DistributionTime { get; set; }
            public string? VenueName { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCycle(string cycleName, string startDate, string endDate)
        {
            if (string.IsNullOrWhiteSpace(cycleName))
            {
                TempData["ErrorMessage"] = "Cycle name is required.";
                return RedirectToAction("Cycles");
            }

            DateOnly? parsedStart = DateOnly.TryParse(startDate, out var s) ? s : null;
            DateOnly? parsedEnd = DateOnly.TryParse(endDate, out var e) ? e : null;

            var cycle = new Foodparcelcycle
            {
                Cyclename = cycleName,
                Startdate = parsedStart,
                Enddate = parsedEnd
            };

            _context.Foodparcelcycles.Add(cycle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "New distribution cycle created successfully.";
            return RedirectToAction("Cycles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCycle(int cycleId, string cycleName, string startDate, string endDate)
        {
            var cycle = await _context.Foodparcelcycles.FindAsync(cycleId);
            if (cycle == null)
            {
                TempData["ErrorMessage"] = "Cycle not found.";
                return RedirectToAction("Cycles");
            }

            DateOnly? parsedStart = DateOnly.TryParse(startDate, out var s) ? s : cycle.Startdate;
            DateOnly? parsedEnd = DateOnly.TryParse(endDate, out var e) ? e : cycle.Enddate;

            cycle.Cyclename = cycleName ?? cycle.Cyclename;
            cycle.Startdate = parsedStart;
            cycle.Enddate = parsedEnd;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cycle details updated successfully.";
            return RedirectToAction("Cycles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelCycle(int cycleId)
        {
            var cycle = await _context.Foodparcelcycles
                .Include(c => c.Cycleitems)
                .Include(c => c.Requests).ThenInclude(r => r.ApplicationStatuses)
                .FirstOrDefaultAsync(c => c.Cycleid == cycleId);

            if (cycle == null)
            {
                TempData["ErrorMessage"] = "Food parcel cycle not found.";
                return RedirectToAction("Cycles");
            }

            try
            {
                if (cycle.Requests != null && cycle.Requests.Count > 0)
                {
                    foreach (var req in cycle.Requests)
                    {
                        if (req.ApplicationStatuses != null && req.ApplicationStatuses.Count > 0)
                            _context.ApplicationStatuses.RemoveRange(req.ApplicationStatuses);
                    }
                    _context.Requests.RemoveRange(cycle.Requests);
                }

                if (cycle.Cycleitems != null && cycle.Cycleitems.Count > 0)
                    _context.Cycleitems.RemoveRange(cycle.Cycleitems);

                _context.Foodparcelcycles.Remove(cycle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Food parcel cycle closed successfully.";
            }
            catch (DbUpdateException)
            {
                _context.ChangeTracker.Clear();
                TempData["ErrorMessage"] = "We could not delete this cycle. It may be linked to existing distributions.";
            }

            return RedirectToAction("Cycles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemToCycle([FromBody] AddItemDto? dto)
        {
            if (dto == null || dto.CycleId <= 0 || string.IsNullOrWhiteSpace(dto.ItemName))
                return Json(new { success = false, message = "Invalid parcel item data." });

            var cycle = await _context.Foodparcelcycles.FindAsync(dto.CycleId);
            if (cycle == null)
                return Json(new { success = false, message = "Cycle not found." });

            var item = new Cycleitem
            {
                Cycleid = dto.CycleId,
                Itemname = dto.ItemName,
                Quantity = dto.Quantity.ToString()
            };

            _context.Cycleitems.Add(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Item added to cycle successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCycleItem([FromBody] UpdateItemDto? dto)
        {
            if (dto == null || dto.ItemId <= 0 || string.IsNullOrWhiteSpace(dto.ItemName))
                return Json(new { success = false, message = "Invalid item data." });

            var item = await _context.Cycleitems.FindAsync(dto.ItemId);
            if (item == null)
                return Json(new { success = false, message = "Parcel item not found." });

            item.Itemname = dto.ItemName;
            item.Quantity = dto.Quantity.ToString();

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Parcel item updated." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var item = await _context.Cycleitems.FindAsync(itemId);
            if (item == null)
                return Json(new { success = false, message = "Item not found." });

            _context.Cycleitems.Remove(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Item removed successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> Distributions()
        {
            var (_, campusId, _) = await GetCurrentAdminCampusAsync();

            var scheduledRequestIds = await _context.Distributionrequests
                .Select(dr => dr.Requestid)
                .ToListAsync();

            var eligibleApplications = await _context.Requests
                .Include(r => r.Student).ThenInclude(s => s!.Person)
                .Include(r => r.Cycle)
                .Include(r => r.ApplicationStatuses)
                .Where(r => r.Student != null && r.Student.Campusid == campusId && !scheduledRequestIds.Contains(r.Requestid))
                .ToListAsync();

            var filteredEligible = eligibleApplications.Where(r =>
            {
                string latestStatus = r.ApplicationStatuses
                    .OrderByDescending(s => s.Applicationstatusid)
                    .Select(s => s.Applicationstatus)
                    .FirstOrDefault() ?? "PENDING";
                return latestStatus == "APPROVED";
            }).Select(r => new DistributionCollectionItemViewModel
            {
                RequestId = r.Requestid,
                StudentNumber = r.Student?.Studentnumber ?? "N/A",
                StudentName = r.Student?.Person != null ? $"{r.Student.Person.Firstname} {r.Student.Person.Lastname}".Trim() : "N/A",
                CampusName = campusId.ToString(),
                CycleName = r.Cycle?.Cyclename ?? "N/A",
                CollectionStatus = "APPROVED"
            }).ToList();

            return View(filteredEligible);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDistribution([FromBody] ScheduleDistributionDto? dto)
        {
            if (dto == null || dto.RequestId <= 0 || string.IsNullOrWhiteSpace(dto.VenueName) || string.IsNullOrWhiteSpace(dto.DistributionDate) || string.IsNullOrWhiteSpace(dto.DistributionTime))
                return Json(new { success = false, message = "Please complete all required scheduling fields." });

            if (!DateTime.TryParse(dto.DistributionDate, out var distDate) || distDate.Date < DateTime.Today)
                return Json(new { success = false, message = "The distribution date cannot be earlier than today." });

            var (admin, campusId, _) = await GetCurrentAdminCampusAsync();
            if (admin == null) return Json(new { success = false, message = "Unauthorized SRC administrator." });

            var request = await _context.Requests
                .Include(r => r.Student)
                .Include(r => r.Cycle)
                .Include(r => r.ApplicationStatuses)
                .Where(r => r.Student != null && r.Student.Campusid == campusId)
                .FirstOrDefaultAsync(r => r.Requestid == dto.RequestId);

            if (request == null)
                return Json(new { success = false, message = "Application not found or outside your campus." });

            string latestStatus = request.ApplicationStatuses
                .OrderByDescending(s => s.Applicationstatusid)
                .Select(s => s.Applicationstatus)
                .FirstOrDefault() ?? "";

            if (latestStatus != "APPROVED")
                return Json(new { success = false, message = "Only approved applications can be scheduled." });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venue = await _context.Venues.FirstOrDefaultAsync(v => v.Venuename == dto.VenueName && v.Campusid == campusId);
                if (venue == null)
                {
                    venue = new Venue { Venuename = dto.VenueName, Campusid = campusId };
                    _context.Venues.Add(venue);
                    await _context.SaveChangesAsync();
                }

                var distribution = new Distribution
                {
                    Distributiondate = DateOnly.FromDateTime(distDate),
                    Distributiontime = dto.DistributionTime,
                    Cycleid = request.Cycleid,
                    Venueid = venue.Venueid,
                    Srcadminid = admin.Srcadminid
                };

                _context.Distributions.Add(distribution);
                await _context.SaveChangesAsync();

                var distRequest = new Distributionrequest
                {
                    Distributionid = distribution.Distributionid,
                    Requestid = request.Requestid
                };
                _context.Distributionrequests.Add(distRequest);

                string refNumber = $"FPS-{request.Requestid:D4}";

                var appStatus = new ApplicationStatus
                {
                    Requestid = request.Requestid,
                    Applicationstatus = "READY_FOR_COLLECTION",
                    DecisionReason = $"Scheduled for {dto.DistributionDate} at {dto.DistributionTime} at {venue.Venuename}",
                    DecisionDate = DateTime.Now,
                    Srcadminid = admin.Srcadminid
                };
                _context.ApplicationStatuses.Add(appStatus);

                var notification = new Notification
                {
                    Studentid = request.Studentid,
                    Distributionid = distribution.Distributionid,
                    Notificationtype = "COLLECTION_READY",
                    Title = "Parcel Ready for Collection",
                    Message = $"Your food parcel ({refNumber}) is ready for collection on {dto.DistributionDate} at {dto.DistributionTime} at {venue.Venuename}.",
                    Isread = "NO",
                    Createddate = DateTime.Now,
                    Srcadminid = admin.Srcadminid
                };
                _context.Notifications.Add(notification);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Distribution scheduled successfully." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "We could not save your distribution schedule. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Collections()
        {
            var (_, campusId, _) = await GetCurrentAdminCampusAsync();

            var scheduledRequests = await _context.Distributionrequests
                .Include(dr => dr.Request).ThenInclude(r => r!.Student).ThenInclude(s => s!.Person)
                .Include(dr => dr.Request).ThenInclude(r => r!.Cycle)
                .Include(dr => dr.Request).ThenInclude(r => r!.ApplicationStatuses)
                .Include(dr => dr.Distribution).ThenInclude(d => d!.Venue)
                .Where(dr => dr.Request != null && dr.Request.Student != null && dr.Request.Student.Campusid == campusId)
                .ToListAsync();

            var collectionItems = scheduledRequests.Select(dr =>
            {
                var req = dr.Request!;
                var dist = dr.Distribution;

                string currentStatus = req.ApplicationStatuses
                    .OrderByDescending(s => s.Applicationstatusid)
                    .Select(s => s.Applicationstatus)
                    .FirstOrDefault() ?? "READY_FOR_COLLECTION";

                return new DistributionCollectionItemViewModel
                {
                    RequestId = req.Requestid,
                    StudentNumber = req.Student?.Studentnumber ?? "N/A",
                    StudentName = req.Student?.Person != null ? $"{req.Student.Person.Firstname} {req.Student.Person.Lastname}".Trim() : "N/A",
                    CycleName = req.Cycle?.Cyclename ?? "N/A",
                    DistributionDate = dist?.Distributiondate?.ToString("yyyy-MM-dd") ?? "N/A",
                    DistributionTime = dist?.Distributiontime ?? "N/A",
                    VenueName = dist?.Venue?.Venuename ?? "N/A",
                    CollectionStatus = currentStatus == "COLLECTED" || currentStatus == "NOT COLLECTED" ? currentStatus : "SCHEDULED",
                    CurrentStatus = currentStatus,
                    ScheduledDate = dist?.Distributiondate != null
                        ? $"{dist.Distributiondate:yyyy-MM-dd} {dist.Distributiontime}"
                        : "TBC"
                };
            }).ToList();

            return View(collectionItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCollectionStatus([FromBody] MarkCollectionDto? dto)
        {
            if (dto == null || dto.RequestId <= 0 || string.IsNullOrEmpty(dto.CollectionStatus))
                return Json(new { success = false, message = "Invalid collection update data." });

            var (admin, campusId, _) = await GetCurrentAdminCampusAsync();
            if (admin == null) return Json(new { success = false, message = "Unauthorized SRC administrator." });

            string statusUpper = dto.CollectionStatus.ToUpper();
            if (statusUpper != "COLLECTED" && statusUpper != "NOT COLLECTED")
                return Json(new { success = false, message = "Invalid collection status value." });

            var request = await _context.Requests
                .Include(r => r.Student).ThenInclude(s => s!.Person)
                .Include(r => r.ApplicationStatuses)
                .Where(r => r.Student != null && r.Student.Campusid == campusId)
                .FirstOrDefaultAsync(r => r.Requestid == dto.RequestId);

            if (request == null)
                return Json(new { success = false, message = "Request not found or outside your campus." });

            bool isScheduled = await _context.Distributionrequests.AnyAsync(dr => dr.Requestid == dto.RequestId);
            if (!isScheduled)
                return Json(new { success = false, message = "This application has not been scheduled for distribution yet." });

            string refNumber = $"FPS-{request.Requestid:D4}";

            var status = new ApplicationStatus
            {
                Requestid = dto.RequestId,
                Applicationstatus = statusUpper,
                DecisionReason = statusUpper == "COLLECTED" ? "Marked as COLLECTED by SRC Admin" : "Marked as NOT COLLECTED by SRC Admin",
                DecisionDate = DateTime.Now,
                Srcadminid = admin.Srcadminid
            };

            _context.ApplicationStatuses.Add(status);

            _context.Notifications.Add(new Notification
            {
                Srcadminid = admin.Srcadminid,
                Notificationtype = "COLLECTION_LOG",
                Title = $"Collection Logged {refNumber}",
                Message = $"{refNumber} for {request.Student?.Person?.Firstname} {request.Student?.Person?.Lastname} marked as {statusUpper}.",
                Isread = "NO",
                Createddate = DateTime.Now
            });

            await _context.SaveChangesAsync();

            string successMsg = statusUpper == "COLLECTED"
                ? "Collection recorded successfully. The student has been marked as COLLECTED."
                : "The collection status has been updated to NOT COLLECTED.";

            return Json(new { success = true, message = successMsg });
        }

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var (admin, _, _) = await GetCurrentAdminCampusAsync();
            if (admin == null) return RedirectToAction("Dashboard");

            var notifications = await _context.Notifications
                .Where(n => n.Srcadminid == admin.Srcadminid || n.Srcadminid == null)
                .OrderByDescending(n => n.Createddate)
                .ToListAsync();

            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Username == username);

            if (person == null) return RedirectToAction("Dashboard");

            var srcAdmin = await _context.Srcadmins
                .Include(s => s.Campus).ThenInclude(c => c!.University)
                .FirstOrDefaultAsync(s => s.Personid == person.Personid);

            var model = new SrcProfileViewModel
            {
                PersonId = person.Personid,
                SrcAdminId = srcAdmin?.Srcadminid ?? 0,
                Username = person.Username,
                Firstname = person.Firstname ?? "",
                Lastname = person.Lastname ?? "",
                Emailaddress = person.Emailaddress ?? "",
                Phonenumber = person.Phonenumber ?? "",
                Gender = person.Gender ?? "",
                Dateofbirth = person.Dateofbirth?.ToString("yyyy-MM-dd") ?? "",
                CampusName = srcAdmin?.Campus?.Campusname ?? "Main Campus",
                UniversityName = srcAdmin?.Campus?.University?.Universityname ?? "University",
                TermStart = srcAdmin?.Termstart ?? "N/A",
                TermEnd = srcAdmin?.Termend ?? "N/A"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(SrcProfileViewModel model)
        {
            var person = await _context.Persons.FindAsync(model.PersonId);
            if (person == null) return RedirectToAction("Profile");

            person.Firstname = model.Firstname;
            person.Lastname = model.Lastname;
            person.Emailaddress = model.Emailaddress;
            person.Phonenumber = model.Phonenumber;
            person.Gender = model.Gender;

            if (!string.IsNullOrWhiteSpace(model.Dateofbirth) && DateOnly.TryParse(model.Dateofbirth, out var dob))
            {
                person.Dateofbirth = dob;
            }

            await _context.SaveChangesAsync();
            model.SuccessMessage = "Profile updated successfully.";

            return View(model);
        }

        public class UpdateStatusDto
        {
            public int RequestId { get; set; }
            public string? NewStatus { get; set; }
            public string? Remarks { get; set; }
        }

        public class AddItemDto
        {
            public int CycleId { get; set; }
            public string? ItemName { get; set; }
            public int Quantity { get; set; } = 1;
        }

        public class UpdateItemDto
        {
            public int ItemId { get; set; }
            public string? ItemName { get; set; }
            public int Quantity { get; set; } = 1;
        }
    }
}