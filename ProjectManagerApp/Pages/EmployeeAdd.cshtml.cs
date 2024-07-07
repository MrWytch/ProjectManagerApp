using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Stránka EmployeeAdd slouží k pøidání nového zamìstnance do tabulky databáze.
    /// </summary>
    public class EmployeeAddModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EmployeeAddModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Employees.Add(Employee);
            _context.SaveChanges();

            TempData["EmployeeAdded"] = "Zaznam byl uspesne pridan."; // Pro js alert o úspìchu na stránce HumanResources

            return RedirectToPage("./HumanResources"); // Pøesmìrování na seznam zamìstnancù po úspìšném pøidání
        }
    }
}
