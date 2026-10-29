using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentFoodParcelApp.Models;

namespace StudentFoodParcelApp.Controllers
{
    [Authorize(Roles = "ITAdmin")]
    public class PersonsController : Controller
    {
        private readonly StudentFoodParcelDbContext _context;

        public PersonsController(StudentFoodParcelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Persons.ToListAsync());
        }

        public async Task<IActionResult> Details(int? personid)
        {
            if (personid == null) return NotFound("The requested record could not be found.");

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Personid == personid);
            if (person == null) return NotFound("The requested record could not be found.");

            return View(person);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Personid,Firstname,Lastname,Emailaddress,Phonenumber,Gender,Dateofbirth,Accountstatus,Createddate,Lastlogin,Username,Password")] Person person)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(person);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    _context.ChangeTracker.Clear();
                    ModelState.AddModelError("", "We could not save your changes. Please try again.");
                }
            }
            return View(person);
        }

        public async Task<IActionResult> Edit(int? personid)
        {
            if (personid == null) return NotFound("The requested record could not be found.");

            var person = await _context.Persons.FindAsync(personid);
            if (person == null) return NotFound("The requested record could not be found.");
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? personid, [Bind("Personid,Firstname,Lastname,Emailaddress,Phonenumber,Gender,Dateofbirth,Accountstatus,Createddate,Lastlogin,Username,Password")] Person person)
        {
            if (personid != person.Personid) return NotFound("The requested record could not be found.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Personid)) return NotFound("The requested record could not be found.");
                    ModelState.AddModelError("", "This record was modified by another user. Refresh the page to view the latest information.");
                }
                catch (DbUpdateException)
                {
                    _context.ChangeTracker.Clear();
                    ModelState.AddModelError("", "We could not save your changes. Please try again.");
                }
            }
            return View(person);
        }

        public async Task<IActionResult> Delete(int? personid)
        {
            if (personid == null) return NotFound("The requested record could not be found.");

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Personid == personid);
            if (person == null) return NotFound("The requested record could not be found.");

            return View(person);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? personid)
        {
            var person = await _context.Persons.FindAsync(personid);
            if (person != null)
            {
                try
                {
                    _context.Persons.Remove(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    _context.ChangeTracker.Clear();
                    TempData["ErrorMessage"] = "We could not remove this person because dependent records exist.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int? personid)
        {
            return _context.Persons.Any(e => e.Personid == personid);
        }
    }
}