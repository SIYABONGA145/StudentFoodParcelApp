using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentFoodParcelApp.Models;

namespace StudentFoodParcelApp.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly StudentFoodParcelDbContext _context;

        public StudentController(StudentFoodParcelDbContext context)
        {
            _context = context;
        }

        private async Task<Student?> GetCurrentStudentAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return null;

            return await _context.Students
                .Include(s => s.Person)
                .Include(s => s.Campus)
                .FirstOrDefaultAsync(s => s.Person != null && s.Person.Username == username);
        }

        private static string LatestStatus(Request r)
        {
            if (r.ApplicationStatuses == null || !r.ApplicationStatuses.Any())
                return "PENDING";

            return r.ApplicationStatuses
                .OrderByDescending(a => a.Applicationstatusid)
                .FirstOrDefault()?.Applicationstatus ?? "PENDING";
        }

        private static int ParseQuantity(string? q)
            => int.TryParse(q, out var n) && n > 0 ? n : 1;

        private static string FormatDate(DateOnly? d)
            => d.HasValue ? d.Value.ToString("dd MMM yyyy") : "TBC";

        private static string BuildToken(int requestId) => $"FPS-{requestId:D6}";

        private static bool IsUnread(string? isread)
            => !string.Equals(isread, "YES", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(isread, "true", StringComparison.OrdinalIgnoreCase)
            && isread != "1";

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Challenge();

            var requests = await _context.Requests
                .Where(r => r.Studentid == student.Studentid)
                .Include(r => r.Cycle).ThenInclude(c => c.Cycleitems)
                .Include(r => r.ApplicationStatuses)
                .OrderByDescending(r => r.Requestid)
                .ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var activeCycles = await _context.Foodparcelcycles
                .Where(c => c.Status == "OPEN"
                         || (c.Startdate <= today && c.Enddate >= today))
                .Include(c => c.Cycleitems)
                .OrderBy(c => c.Enddate)
                .Take(3)
                .ToListAsync();

            var recentNotifications = await _context.Notifications
                .Where(n => n.Studentid == student.Studentid)
                .OrderByDescending(n => n.Createddate)
                .Take(3)
                .ToListAsync();

            var unreadCount = recentNotifications.Count(n => IsUnread(n.Isread));

            var nextApproved = requests
                .Where(r => LatestStatus(r) == "APPROVED")
                .OrderBy(r => r.Cycle?.Startdate)
                .FirstOrDefault();

            CollectionViewModel? nextCollection = null;
            if (nextApproved != null)
            {
                var distReq = await _context.Distributionrequests
                    .Include(dr => dr.Distribution).ThenInclude(d => d.Venue)
                    .FirstOrDefaultAsync(dr => dr.Requestid == nextApproved.Requestid);

                nextCollection = new CollectionViewModel
                {
                    RequestId = nextApproved.Requestid,
                    CycleName = nextApproved.Cycle?.Cyclename ?? "N/A",
                    CampusName = student.Campus?.Campusname ?? "N/A",
                    VenueName = distReq?.Distribution?.Venue?.Venuename ?? "TBC",
                    DistributionDate = FormatDate(distReq?.Distribution?.Distributiondate),
                    DistributionTime = distReq?.Distribution?.Distributiontime ?? "TBC",
                    VerificationToken = BuildToken(nextApproved.Requestid)
                };
            }

            var vm = new StudentDashboardViewModel
            {
                StudentNumber = student.Studentnumber,
                StudentName = student.Person != null
                    ? $"{student.Person.Firstname} {student.Person.Lastname}".Trim()
                    : "Student",
                CampusName = student.Campus?.Campusname ?? "N/A",

                TotalApplications = requests.Count,
                PendingCount = requests.Count(r => LatestStatus(r) == "PENDING"),
                ApprovedCount = requests.Count(r => LatestStatus(r) == "APPROVED"),
                CollectedCount = requests.Count(r => LatestStatus(r) == "COLLECTED"),
                RejectedCount = requests.Count(r => LatestStatus(r) == "REJECTED"),

                RecentApplications = requests.Take(5).Select(r => new StudentApplicationViewModel
                {
                    RequestId = r.Requestid,
                    CycleId = r.Cycleid ?? 0,
                    CycleName = r.Cycle?.Cyclename ?? "N/A",
                    RequestDate = r.Requestdate ?? "",
                    CurrentStatus = LatestStatus(r),
                    Remarks = r.ApplicationStatuses
                        .OrderByDescending(a => a.Applicationstatusid)
                        .FirstOrDefault()?.DecisionReason,
                    CycleItems = (r.Cycle?.Cycleitems ?? Enumerable.Empty<Cycleitem>())
                        .Select(ci => new ParcelItemViewModel
                        {
                            ItemName = ci.Itemname,
                            Quantity = ParseQuantity(ci.Quantity)
                        }).ToList()
                }).ToList(),

                ActiveCycles = activeCycles.Select(c => new ParcelCycleViewModel
                {
                    CycleId = c.Cycleid,
                    CycleName = c.Cyclename,
                    StartDate = FormatDate(c.Startdate),
                    EndDate = FormatDate(c.Enddate),
                    IsActive = true,
                    Items = c.Cycleitems.Select(ci => new ParcelItemViewModel
                    {
                        ItemName = ci.Itemname,
                        Quantity = ParseQuantity(ci.Quantity)
                    }).ToList()
                }).ToList(),

                RecentNotifications = recentNotifications,
                UnreadNotifications = unreadCount,
                NextCollection = nextCollection
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Applications()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Challenge();

            var requests = await _context.Requests
                .Where(r => r.Studentid == student.Studentid)
                .Include(r => r.Cycle).ThenInclude(c => c.Cycleitems)
                .Include(r => r.ApplicationStatuses)
                .OrderByDescending(r => r.Requestid)
                .ToListAsync();

            var apps = requests.Select(r => new StudentApplicationViewModel
            {
                RequestId = r.Requestid,
                CycleId = r.Cycleid ?? 0,
                CycleName = r.Cycle?.Cyclename ?? "N/A",
                RequestDate = r.Requestdate ?? "",
                CurrentStatus = LatestStatus(r),
                Remarks = r.ApplicationStatuses
                    .OrderByDescending(a => a.Applicationstatusid)
                    .FirstOrDefault()?.DecisionReason,
                CycleItems = (r.Cycle?.Cycleitems ?? Enumerable.Empty<Cycleitem>())
                    .Select(ci => new ParcelItemViewModel
                    {
                        ItemName = ci.Itemname,
                        Quantity = ParseQuantity(ci.Quantity)
                    }).ToList()
            }).ToList();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var activeCycles = await _context.Foodparcelcycles
                .Where(c => c.Status == "OPEN"
                         || (c.Startdate <= today && c.Enddate >= today))
                .OrderBy(c => c.Enddate)
                .ToListAsync();

            ViewBag.ActiveCycles = activeCycles;
            ViewBag.ExistingCycleIds = apps.Select(a => a.CycleId).ToList();
            ViewBag.StudentNumber = student.Studentnumber;
            ViewBag.StudentName = student.Person != null
                ? $"{student.Person.Firstname} {student.Person.Lastname}".Trim()
                : "Student";

            return View(apps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitApplication(int cycleId)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            var cycle = await _context.Foodparcelcycles
                .FirstOrDefaultAsync(c => c.Cycleid == cycleId);

            if (cycle == null)
                return Json(new { success = false, message = "Selected cycle no longer exists." });

            if (await _context.Requests.AnyAsync(r =>
                r.Studentid == student.Studentid && r.Cycleid == cycleId))
                return Json(new { success = false, message = "You already have an application for this cycle." });

            var request = new Request
            {
                Studentid = student.Studentid,
                Cycleid = cycleId,
                Requestdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            _context.ApplicationStatuses.Add(new ApplicationStatus
            {
                Requestid = request.Requestid,
                Applicationstatus = "PENDING",
                DecisionDate = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithdrawApplication(int requestId)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Challenge();

            var request = await _context.Requests
                .Include(r => r.ApplicationStatuses)
                .FirstOrDefaultAsync(r => r.Requestid == requestId && r.Studentid == student.Studentid);

            if (request == null)
            {
                TempData["ErrorMessage"] = "Application not found.";
                return RedirectToAction(nameof(Applications));
            }

            var status = LatestStatus(request);
            if (status == "COLLECTED" || status == "WITHDRAWN")
            {
                TempData["ErrorMessage"] = "This application cannot be withdrawn.";
                return RedirectToAction(nameof(Applications));
            }

            _context.ApplicationStatuses.Add(new ApplicationStatus
            {
                Requestid = request.Requestid,
                Applicationstatus = "WITHDRAWN",
                DecisionDate = DateTime.Now,
                DecisionReason = "Withdrawn by student"
            });
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Application withdrawn successfully.";
            return RedirectToAction(nameof(Applications));
        }

        [HttpGet]
        public async Task<IActionResult> Collections()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Challenge();

            var requests = await _context.Requests
                .Where(r => r.Studentid == student.Studentid)
                .Include(r => r.Cycle)
                .Include(r => r.ApplicationStatuses)
                .ToListAsync();

            var approved = requests
                .Where(r => LatestStatus(r) == "APPROVED")
                .OrderBy(r => r.Cycle?.Startdate)
                .ToList();

            var vm = new List<CollectionViewModel>();
            foreach (var r in approved)
            {
                var distReq = await _context.Distributionrequests
                    .Include(dr => dr.Distribution).ThenInclude(d => d.Venue)
                    .FirstOrDefaultAsync(dr => dr.Requestid == r.Requestid);

                var token = BuildToken(r.Requestid);

                vm.Add(new CollectionViewModel
                {
                    RequestId = r.Requestid,
                    CycleName = r.Cycle?.Cyclename ?? "N/A",
                    CampusName = student.Campus?.Campusname ?? "N/A",
                    VenueName = distReq?.Distribution?.Venue?.Venuename ?? "TBC",
                    DistributionDate = FormatDate(distReq?.Distribution?.Distributiondate),
                    DistributionTime = distReq?.Distribution?.Distributiontime ?? "TBC",
                    VerificationToken = token,
                    QrCodeBase64 = GenerateQrBase64(token)
                });
            }

            return View(vm);
        }

        private static string GenerateQrBase64(string token)
        {
            try
            {
                using var qrGen = new QRCoder.QRCodeGenerator();
                using var qrData = qrGen.CreateQrCode(token, QRCoder.QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCoder.PngByteQRCode(qrData);
                var bytes = qrCode.GetGraphic(20);
                return "data:image/png;base64," + Convert.ToBase64String(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cycles()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var raw = await _context.Foodparcelcycles
                .Where(c => c.Status == "OPEN"
                         || (c.Startdate <= today && c.Enddate >= today))
                .Include(c => c.Cycleitems)
                .OrderBy(c => c.Enddate)
                .ToListAsync();

            var cycles = raw.Select(c => new ParcelCycleViewModel
            {
                CycleId = c.Cycleid,
                CycleName = c.Cyclename,
                StartDate = FormatDate(c.Startdate),
                EndDate = FormatDate(c.Enddate),
                IsActive = true,
                Items = c.Cycleitems.Select(ci => new ParcelItemViewModel
                {
                    ItemName = ci.Itemname,
                    Quantity = ParseQuantity(ci.Quantity)
                }).ToList()
            }).ToList();

            return View(cycles);
        }

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Challenge();

            var notifications = await _context.Notifications
                .Where(n => n.Studentid == student.Studentid)
                .OrderByDescending(n => n.Createddate)
                .ToListAsync();

            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return Challenge();

            var vm = new StudentProfileViewModel
            {
                PersonId = student.Personid ?? 0,
                StudentId = student.Studentid,
                StudentNumber = student.Studentnumber,
                CampusName = student.Campus?.Campusname ?? "N/A",
                Firstname = student.Person?.Firstname ?? "",
                Lastname = student.Person?.Lastname ?? "",
                Emailaddress = student.Person?.Emailaddress ?? "",
                Phonenumber = student.Person?.Phonenumber,
                Gender = student.Person?.Gender,
                Dateofbirth = student.Person?.Dateofbirth?.ToString("yyyy-MM-dd"),
                Username = student.Person?.Username
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(StudentProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = await _context.Students
                .Include(s => s.Person)
                .Include(s => s.Campus)
                .FirstOrDefaultAsync(s => s.Studentid == model.StudentId);

            if (student == null || student.Person == null)
                return NotFound();

            student.Studentnumber = model.StudentNumber;
            student.Person.Firstname = model.Firstname;
            student.Person.Lastname = model.Lastname;
            student.Person.Emailaddress = model.Emailaddress;
            student.Person.Phonenumber = model.Phonenumber;
            student.Person.Gender = model.Gender;
            student.Person.Dateofbirth = DateOnly.TryParse(model.Dateofbirth, out var dob)
                ? dob
                : null;

            await _context.SaveChangesAsync();

            model.SuccessMessage = "Profile updated successfully.";
            model.CampusName = student.Campus?.Campusname ?? "N/A";
            model.Username = student.Person.Username;
            return View(model);
        }
    }
}