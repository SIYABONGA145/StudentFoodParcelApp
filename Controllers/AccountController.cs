using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentFoodParcelApp.Models;

namespace StudentFoodParcelApp.Controllers;

public class AccountController(StudentFoodParcelDbContext context) : Controller
{
    private readonly StudentFoodParcelDbContext _context = context;

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            return RedirectToRoleDashboard();
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        model.Username = (model.Username ?? string.Empty).Trim();
        var hashedPassword = HashPassword(model.Password);

        var person = await _context.Persons.FirstOrDefaultAsync(p =>
            (p.Username == model.Username || p.Emailaddress == model.Username) &&
            p.Password == hashedPassword);

        if (person == null)
        {
            ModelState.AddModelError("", "Invalid username/email or password.");
            return View(model);
        }

        if (person.Accountstatus != "Active")
        {
            ModelState.AddModelError("", "This account is currently inactive. Please contact support.");
            return View(model);
        }

        string role = "";
        if (await _context.Students.AnyAsync(s => s.Personid == person.Personid)) role = "Student";
        else if (await _context.Srcadmins.AnyAsync(s => s.Personid == person.Personid)) role = "SRCAdmin";
        else if (await _context.Financestaffs.AnyAsync(f => f.Personid == person.Personid)) role = "Finance";
        else if (await _context.Itadmins.AnyAsync(i => i.Personid == person.Personid)) role = "ITAdmin";

        if (string.IsNullOrEmpty(role))
        {
            ModelState.AddModelError("", "No role assigned to this account.");
            return View(model);
        }

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, person.Personid.ToString()),
            new(ClaimTypes.Name, person.Username),
            new(ClaimTypes.GivenName, $"{person.Firstname} {person.Lastname}"),
            new(ClaimTypes.Email, person.Emailaddress ?? ""),
            new(ClaimTypes.Role, role)
        ];

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        person.Lastlogin = DateTime.Now;
        await _context.SaveChangesAsync();

        return RedirectToRoleDashboard(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Universities = new SelectList(_context.Universities, "Universityid", "Universityname");
        ViewBag.Campuses = new SelectList(Enumerable.Empty<SelectListItem>());
        ViewBag.Departments = new SelectList(_context.Departments, "Departmentid", "Departmentname");
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetCampusesByUniversity(int universityId)
    {
        var campuses = await _context.Campuses
            .Where(c => c.Universityid == universityId)
            .Select(c => new { value = c.Campusid, text = c.Campusname })
            .ToListAsync();

        return Json(campuses);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (await _context.Persons.AnyAsync(p => p.Username == model.Username))
        {
            ModelState.AddModelError("Username", "Username is already taken.");
        }

        if (ModelState.IsValid)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DateOnly? dob = null;
                if (DateOnly.TryParse(model.DateOfBirth, out var parsedDob))
                {
                    dob = parsedDob;
                }

                var person = new Person
                {
                    Firstname = model.FirstName.Trim(),
                    Lastname = model.LastName.Trim(),
                    Emailaddress = model.EmailAddress.Trim(),
                    Phonenumber = model.PhoneNumber?.Trim(),
                    Gender = model.Gender,
                    Dateofbirth = dob,
                    Accountstatus = "Active",
                    Createddate = DateTime.Now,
                    Username = model.Username.Trim(),
                    Password = HashPassword(model.Password)
                };

                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                switch (model.Role)
                {
                    case "Student":
                        if (string.IsNullOrEmpty(model.StudentNumber)) throw new Exception("Student Number is required.");
                        _context.Students.Add(new Student { Personid = person.Personid, Campusid = model.CampusId, Studentnumber = model.StudentNumber.Trim() });
                        break;
                    case "Finance":
                        if (!model.DepartmentId.HasValue) throw new Exception("Department is required.");
                        _context.Financestaffs.Add(new Financestaff { Personid = person.Personid, Campusid = model.CampusId, Departmentid = model.DepartmentId.Value });
                        break;
                    case "SRCAdmin":
                        _context.Srcadmins.Add(new Srcadmin { Personid = person.Personid, Campusid = model.CampusId, Termstart = model.TermStart ?? DateTime.Now.ToString("yyyy-01-01"), Termend = model.TermEnd ?? DateTime.Now.ToString("yyyy-12-31") });
                        break;
                    case "ITAdmin":
                        _context.Itadmins.Add(new Itadmin { Personid = person.Personid, Campusid = model.CampusId });
                        break;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Account created successfully! Please log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "We could not complete your registration. " + ex.Message);
            }
        }

        ViewBag.Universities = new SelectList(_context.Universities, "Universityid", "Universityname", model.UniversityId);
        ViewBag.Campuses = new SelectList(_context.Campuses.Where(c => c.Universityid == model.UniversityId), "Campusid", "Campusname", model.CampusId);
        ViewBag.Departments = new SelectList(_context.Departments, "Departmentid", "Departmentname", model.DepartmentId);
        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Emailaddress == model.EmailAddress);
            if (person != null) return RedirectToAction("ResetPassword", new { email = model.EmailAddress });
            ModelState.AddModelError("", "No account found associated with that email address.");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        return View(new ResetPasswordViewModel { EmailAddress = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Emailaddress == model.EmailAddress);
            if (person != null)
            {
                person.Password = HashPassword(model.NewPassword);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Password reset successfully. Please login.";
                return RedirectToAction("Login");
            }
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        var personIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(personIdClaim) || !int.TryParse(personIdClaim, out int personId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
        {
            TempData["ErrorMessage"] = "New passwords do not match or are empty.";
            return RedirectToRoleProfile();
        }

        var person = await _context.Persons.FindAsync(personId);
        if (person == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (person.Password != HashPassword(currentPassword))
        {
            TempData["ErrorMessage"] = "Current password is incorrect.";
            return RedirectToRoleProfile();
        }

        person.Password = HashPassword(newPassword);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Password updated successfully!";
        return RedirectToRoleProfile();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteAccount()
    {
        var personIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(personIdClaim) || !int.TryParse(personIdClaim, out int personId))
        {
            return RedirectToAction("Login", "Account");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Personid == personId);
            var finance = await _context.Financestaffs.FirstOrDefaultAsync(f => f.Personid == personId);
            var src = await _context.Srcadmins.FirstOrDefaultAsync(s => s.Personid == personId);
            var it = await _context.Itadmins.FirstOrDefaultAsync(i => i.Personid == personId);

            if (student != null)
            {
                var studentNum = student.Studentnumber;
                var notifications = await _context.Notifications
                    .Where(n => n.Studentid == student.Studentid || (studentNum != null && n.Message != null && n.Message.Contains(studentNum)))
                    .ToListAsync();
                if (notifications.Count > 0) _context.Notifications.RemoveRange(notifications);

                var fundings = await _context.Fundingstatuses
                    .Where(f => f.Studentid == student.Studentid)
                    .ToListAsync();
                if (fundings.Count > 0) _context.Fundingstatuses.RemoveRange(fundings);

                var requestIds = await _context.Requests
                    .Where(r => r.Studentid == student.Studentid)
                    .Select(r => r.Requestid)
                    .ToListAsync();

                if (requestIds.Count > 0)
                {
                    var appStatuses = await _context.ApplicationStatuses
                        .Where(a => a.Requestid.HasValue && requestIds.Contains(a.Requestid.Value))
                        .ToListAsync();
                    if (appStatuses.Count > 0) _context.ApplicationStatuses.RemoveRange(appStatuses);

                    var requests = await _context.Requests
                        .Where(r => requestIds.Contains(r.Requestid))
                        .ToListAsync();
                    _context.Requests.RemoveRange(requests);
                }

                _context.Students.Remove(student);
            }

            if (finance != null)
            {
                var fundingAudits = await _context.Fundingstatuses
                    .Where(f => f.Financestaffid == finance.Financestaffid)
                    .ToListAsync();
                foreach (var fa in fundingAudits) { fa.Financestaffid = null; }
                _context.Financestaffs.Remove(finance);
            }

            if (src != null)
            {
                var appStatuses = await _context.ApplicationStatuses
                    .Where(a => a.Srcadminid == src.Srcadminid)
                    .ToListAsync();
                foreach (var appStatus in appStatuses) { appStatus.Srcadminid = null; }

                var cycles = await _context.Foodparcelcycles
                    .Where(c => c.Srcadminid == src.Srcadminid)
                    .ToListAsync();
                foreach (var c in cycles) { c.Srcadminid = null; }

                var distributions = await _context.Distributions
                    .Where(d => d.Srcadminid == src.Srcadminid)
                    .ToListAsync();
                foreach (var d in distributions) { d.Srcadminid = null; }

                var srcNotifications = await _context.Notifications
                    .Where(n => n.Srcadminid == src.Srcadminid)
                    .ToListAsync();
                foreach (var sn in srcNotifications) { sn.Srcadminid = null; }

                _context.Srcadmins.Remove(src);
            }

            if (it != null) { _context.Itadmins.Remove(it); }

            var person = await _context.Persons.FindAsync(personId);
            if (person != null) { _context.Persons.Remove(person); }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "Your account has been deleted successfully.";
            return RedirectToAction("Login", "Account");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Failed to delete account. We could not complete your request. Please try again later.";
            return RedirectToRoleProfile();
        }
    }

    private RedirectToActionResult RedirectToRoleProfile()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        return role switch
        {
            "Student" => RedirectToAction("Profile", "Student"),
            "Finance" => RedirectToAction("Profile", "Finance"),
            "SRCAdmin" => RedirectToAction("Profile", "SrcAdmin"),
            "ITAdmin" => RedirectToAction("Profile", "ItAdmin"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    private RedirectToActionResult RedirectToRoleDashboard(string? role = null)
    {
        role ??= User.FindFirstValue(ClaimTypes.Role);

        return role switch
        {
            "Student" => RedirectToAction("Index", "Student"),
            "Finance" => RedirectToAction("Index", "Finance"),
            "SRCAdmin" => RedirectToAction("Dashboard", "SrcAdmin"),
            "ITAdmin" => RedirectToAction("Index", "ItAdmin"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexStringLower(bytes);
    }
}