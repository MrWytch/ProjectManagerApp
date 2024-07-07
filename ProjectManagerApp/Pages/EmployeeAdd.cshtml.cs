using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Str�nka EmployeeAdd slou�� k p�id�n� nov�ho zam�stnance do tabulky datab�ze.
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

            TempData["EmployeeAdded"] = "Zaznam byl uspesne pridan."; // Pro js alert o �sp�chu na str�nce HumanResources

            return RedirectToPage("./HumanResources"); // P�esm�rov�n� na seznam zam�stnanc� po �sp�n�m p�id�n�
        }
    }
}
