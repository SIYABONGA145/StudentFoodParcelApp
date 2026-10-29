using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentFoodParcelApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentFoodParcelApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly StudentFoodParcelDbContext _context;

        public HomeController(StudentFoodParcelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int personId = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : 0;

            if (User.IsInRole("Student") || await _context.Students.AnyAsync(s => s.Personid == personId))
            {
                return RedirectToAction("Index", "Student");
            }

            if (User.IsInRole("Finance") || await _context.Financestaffs.AnyAsync(f => f.Personid == personId))
            {
                return RedirectToAction("Index", "Finance");
            }

            // Route SRC Admin directly to the new Dashboard action
            return RedirectToAction("Dashboard", "SrcAdmin");
        }
    }
}