using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentFoodParcelApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentFoodParcelApp.Controllers
{
    [Authorize(Roles = "Finance,SRCAdmin")]
    public class FinanceController : Controller
    {
        private readonly StudentFoodParcelDbContext _context;

        public FinanceController(StudentFoodParcelDbContext context)
        {
            _context = context;
        }

        private async Task<Financestaff?> GetCurrentFinanceStaffAsync()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int personId))
                return null;

            return await _context.Financestaffs
                .Include(f => f.Person)
                .Include(f => f.Campus)
                .FirstOrDefaultAsync(f => f.Personid == personId);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var staff = await GetCurrentFinanceStaffAsync();
            if (staff == null) return NotFound("You are not authorised to access student verification.");

            var campusStudentIds = await _context.Students
                .Where(s => s.Campusid == staff.Campusid)
                .Select(s => s.Studentid)
                .ToListAsync();

            var latestFunding = await _context.Fundingstatuses
                .Where(f => f.Studentid.HasValue && campusStudentIds.Contains(f.Studentid.Value))
                .GroupBy(f => f.Studentid)
                .Select(g => g.OrderByDescending(x => x.Fundingstatusid).FirstOrDefault())
                .ToListAsync();

            ViewBag.UnreadNotifications = await _context.Notifications
                .CountAsync(n => n.Notificationtype == "FINANCE_VERIFICATION_REQUIRED"
                              && n.Isread == "NO"
                              && n.Studentid.HasValue
                              && campusStudentIds.Contains(n.Studentid.Value));

            ViewBag.CampusName = staff.Campus?.Campusname ?? "Assigned Campus";
            ViewBag.TotalStudents = campusStudentIds.Count;
            ViewBag.FundedCount = latestFunding.Count(f => f != null && f.Fundingstatus1 == "FUNDED");
            ViewBag.UnfundedCount = latestFunding.Count(f => f != null && f.Fundingstatus1 == "UNFUNDED");
            ViewBag.UnverifiedCount = campusStudentIds.Count - latestFunding.Count;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var staff = await GetCurrentFinanceStaffAsync();
            if (staff == null) return NotFound("Finance record not found.");

            var campusStudentIds = await _context.Students
                .Where(s => s.Campusid == staff.Campusid)
                .Select(s => s.Studentid)
                .ToListAsync();

            var notifications = await _context.Notifications
                .Include(n => n.Student).ThenInclude(s => s!.Person)
                .Where(n => n.Notificationtype == "FINANCE_VERIFICATION_REQUIRED"
                         && n.Studentid.HasValue
                         && campusStudentIds.Contains(n.Studentid.Value))
                .OrderByDescending(n => n.Createddate)
                .ToListAsync();

            var unread = notifications.Where(n => n.Isread == "NO").ToList();
            if (unread.Any())
            {
                foreach (var n in unread)
                {
                    n.Isread = "YES";
                    n.Readdate = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }

            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var staff = await GetCurrentFinanceStaffAsync();
            if (staff == null || staff.Person == null)
                return NotFound("Finance staff record not found.");

            var model = new SrcProfileViewModel
            {
                PersonId = staff.Person.Personid,
                Firstname = staff.Person.Firstname ?? "",
                Lastname = staff.Person.Lastname ?? "",
                Emailaddress = staff.Person.Emailaddress ?? "",
                Phonenumber = staff.Person.Phonenumber,
                Gender = staff.Person.Gender,
                Dateofbirth = staff.Person.Dateofbirth?.ToString("yyyy-MM-dd"),
                Username = staff.Person.Username,
                CampusName = staff.Campus?.Campusname ?? "N/A"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(SrcProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var person = await _context.Persons.FindAsync(model.PersonId);
            if (person == null) return NotFound("Person profile not found.");

            person.Firstname = model.Firstname?.Trim() ?? "";
            person.Lastname = model.Lastname?.Trim() ?? "";
            person.Emailaddress = model.Emailaddress?.Trim() ?? "";
            person.Phonenumber = model.Phonenumber?.Trim();
            person.Gender = model.Gender;

            if (!string.IsNullOrWhiteSpace(model.Dateofbirth)
                && DateOnly.TryParse(model.Dateofbirth, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
            {
                person.Dateofbirth = dob;
            }

            try
            {
                _context.Update(person);
                await _context.SaveChangesAsync();
                model.SuccessMessage = "Profile updated successfully!";
            }
            catch (DbUpdateException)
            {
                _context.ChangeTracker.Clear();
                ModelState.AddModelError("", "We could not save your changes. Please try again.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Students(int? cycleId)
        {
            var staff = await GetCurrentFinanceStaffAsync();
            if (staff == null) return NotFound("You are not authorised to access student verification.");

            var campusCycles = await _context.Foodparcelcycles
                .Where(c => c.Requests.Any(r => r.Student != null && r.Student.Campusid == staff.Campusid))
                .OrderByDescending(c => c.Cycleid)
                .ToListAsync();

            var model = new FinanceStudentsViewModel
            {
                AvailableCycles = campusCycles.Select(c => new SelectListItem
                {
                    Value = c.Cycleid.ToString(),
                    Text = $"{c.Cyclename} (Ends: {c.Enddate?.ToString("yyyy-MM-dd") ?? "N/A"})"
                }).ToList(),
                SelectedCycleId = cycleId
            };

            if (cycleId.HasValue)
            {
                var requests = await _context.Requests
                    .Include(r => r.Student).ThenInclude(s => s!.Person)
                    .Include(r => r.Student).ThenInclude(s => s!.Campus)
                    .Where(r => r.Cycleid == cycleId && r.Student != null && r.Student.Campusid == staff.Campusid)
                    .ToListAsync();

                var studentIds = requests
                    .Where(r => r.Student != null)
                    .Select(r => r.Studentid)
                    .ToList();

                var fundingRecords = await _context.Fundingstatuses
                    .Where(f => f.Studentid.HasValue && studentIds.Contains(f.Studentid.Value))
                    .ToListAsync();

                var latestFunding = fundingRecords
                    .GroupBy(f => f.Studentid)
                    .ToDictionary(
                        g => g.Key!.Value,
                        g => g.OrderByDescending(x => x.Fundingstatusid).First());

                var studentsList = new List<StudentFundingVerificationViewModel>();

                foreach (var req in requests)
                {
                    var student = req.Student;
                    if (student == null) continue;

                    latestFunding.TryGetValue(student.Studentid, out var funding);

                    bool isVerified = funding != null;
                    bool isFunded = funding != null && funding.Fundingstatus1 == "FUNDED";

                    studentsList.Add(new StudentFundingVerificationViewModel
                    {
                        StudentId = student.Studentid,
                        StudentNumber = student.Studentnumber ?? "N/A",
                        StudentName = student.Person != null
                            ? $"{student.Person.Firstname} {student.Person.Lastname}".Trim()
                            : "N/A",
                        Email = student.Person?.Emailaddress ?? "N/A",
                        CampusName = student.Campus?.Campusname ?? "N/A",
                        IsVerified = isVerified,
                        IsFunded = isFunded,
                        Reason = funding?.Reason ?? ""
                    });
                }

                model.Students = studentsList
                    .OrderBy(s => s.IsVerified)
                    .ThenBy(s => s.StudentName)
                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFundingChanges([FromBody] List<StudentFundingUpdateModel>? updates)
        {
            if (updates == null || !updates.Any())
                return Json(new { success = false, message = "Please complete the required verification checks before continuing." });

            var staff = await GetCurrentFinanceStaffAsync();
            if (staff == null)
                return Json(new { success = false, message = "You are not authorised to access student verification." });

            int financeStaffId = staff.Financestaffid;
            var srcAdmin = await _context.Srcadmins
                .FirstOrDefaultAsync(s => s.Campusid == staff.Campusid);

            var nowStr = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                foreach (var item in updates)
                {
                    var student = await _context.Students
                        .FirstOrDefaultAsync(s => s.Studentid == item.StudentId && s.Campusid == staff.Campusid);

                    if (student == null) continue;

                    string reason = item.Reason ?? "";
                    if (reason.Length > 255) reason = reason.Substring(0, 255);

                    string statusText = item.IsFunded ? "FUNDED" : "UNFUNDED";

                    var existing = await _context.Fundingstatuses
                        .Where(f => f.Studentid == item.StudentId)
                        .OrderByDescending(f => f.Fundingstatusid)
                        .FirstOrDefaultAsync();

                    bool unchanged = existing != null
                        && existing.Fundingstatus1 == statusText
                        && (existing.Reason ?? "") == reason;

                    if (unchanged) continue;

                    if (existing != null)
                    {
                        existing.Fundingstatus1 = statusText;
                        existing.Reason = reason;
                        existing.Updateddate = nowStr;
                        existing.Financestaffid = financeStaffId;
                        _context.Fundingstatuses.Update(existing);
                    }
                    else
                    {
                        _context.Fundingstatuses.Add(new Fundingstatus
                        {
                            Studentid = item.StudentId,
                            Fundingstatus1 = statusText,
                            Reason = reason,
                            Updateddate = nowStr,
                            Financestaffid = financeStaffId
                        });
                    }

                    string studentMsg = $"Your student information has been verified. Funding Status: {statusText}.";
                    if (studentMsg.Length > 255) studentMsg = studentMsg.Substring(0, 255);

                    _context.Notifications.Add(new Notification
                    {
                        Studentid = item.StudentId,
                        Notificationtype = "FUNDING_UPDATE",
                        Title = "Funding Status Verified",
                        Message = studentMsg,
                        Isread = "NO",
                        Createddate = DateTime.Now
                    });

                    if (srcAdmin != null)
                    {
                        _context.Notifications.Add(new Notification
                        {
                            Srcadminid = srcAdmin.Srcadminid,
                            Notificationtype = "STUDENT_FUNDING_CHANGED",
                            Title = "Student Funding Updated",
                            Message = $"Finance updated Student {student.Studentnumber} to {statusText}.",
                            Isread = "NO",
                            Createddate = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Student verification completed successfully. Applications are ready for SRC review." });
            }
            catch (DbUpdateException)
            {
                _context.ChangeTracker.Clear();
                return Json(new { success = false, message = "The verification result could not be saved. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadStudentList(int? cycleId)
        {
            var staff = await GetCurrentFinanceStaffAsync();
            if (staff == null) return NotFound("You are not authorised to access student verification.");

            if (!cycleId.HasValue) return RedirectToAction("Students");

            var requests = await _context.Requests
                .Include(r => r.Student).ThenInclude(s => s!.Person)
                .Include(r => r.Student).ThenInclude(s => s!.Campus)
                .Where(r => r.Cycleid == cycleId && r.Student != null && r.Student.Campusid == staff.Campusid)
                .ToListAsync();

            var studentIds = requests
                .Where(r => r.Student != null)
                .Select(r => r.Studentid)
                .ToList();

            var fundingRecords = await _context.Fundingstatuses
                .Where(f => f.Studentid.HasValue && studentIds.Contains(f.Studentid.Value))
                .ToListAsync();

            var latestFunding = fundingRecords
                .GroupBy(f => f.Studentid)
                .ToDictionary(
                    g => g.Key!.Value,
                    g => g.OrderByDescending(x => x.Fundingstatusid).First());

            var sb = new StringBuilder();
            sb.AppendLine("Student ID,Student Number,Full Name,Email,Campus,Funding Status,Reason");

            foreach (var req in requests)
            {
                var s = req.Student;
                if (s == null) continue;

                latestFunding.TryGetValue(s.Studentid, out var f);

                string name = s.Person != null
                    ? $"{s.Person.Firstname} {s.Person.Lastname}".Trim()
                    : "N/A";
                string email = s.Person?.Emailaddress ?? "N/A";
                string campus = s.Campus?.Campusname ?? "N/A";
                string status = f?.Fundingstatus1 ?? "UNVERIFIED";
                string reason = (f?.Reason ?? "").Replace(",", " ");

                sb.AppendLine($"{s.Studentid},{s.Studentnumber},{name},{email},{campus},{status},{reason}");
            }

            string safeCampus = (staff.Campus?.Campusname ?? "Campus").Replace(" ", "_");
            var cycle = await _context.Foodparcelcycles.FindAsync(cycleId);
            string safeCycle = (cycle?.Cyclename ?? "Cycle").Replace(" ", "_");

            return File(
                Encoding.UTF8.GetBytes(sb.ToString()),
                "text/csv",
                $"Student_Funding_List_{safeCampus}_{safeCycle}_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}