using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Str�nka EmployeeDetails slou�� k zobrazen� detail� vybran�ho zam�stnance (karta zam�stnance).
    /// </summary>
    public class EmployeeDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EmployeeDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await _context.Employees
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (Employee == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
