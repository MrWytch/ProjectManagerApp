using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Stránka EmployeeEdit slouží k editaci vlastností zamìstnance
    /// </summary>
    public class EmployeeEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EmployeeEditModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [BindProperty]
        public Employee? Employee { get; set; } // Editovaný zamìstnanec
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await _context.Employees.FindAsync(id);

            if (Employee == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Employee).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Projects.Any(e => e.Id == Employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                // Zpracování ostatních možných výjimek z databáze
                ModelState.AddModelError("", "Nastala chyba pøi ukládání zmìn.");
                return Page();
            }

            return RedirectToPage("./EmployeeDetails", new { id = Employee.Id });
        }
    }
}
