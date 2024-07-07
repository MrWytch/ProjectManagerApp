using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Str�nka EmployeeEdit slou�� k editaci vlastnost� zam�stnance
    /// </summary>
    public class EmployeeEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EmployeeEditModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [BindProperty]
        public Employee? Employee { get; set; } // Editovan� zam�stnanec
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
                // Zpracov�n� ostatn�ch mo�n�ch v�jimek z datab�ze
                ModelState.AddModelError("", "Nastala chyba p�i ukl�d�n� zm�n.");
                return Page();
            }

            return RedirectToPage("./EmployeeDetails", new { id = Employee.Id });
        }
    }
}
