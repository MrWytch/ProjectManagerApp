using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Stránka ProjectDetails slouží k zobrazení detailù zvoleného projektu (karta projektu).
    /// </summary>
    public class ProjectDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Project? Project { get; set; } // Projekt, jehož detaily se mají zobrazit
        public IList<Employee> Employees { get; set; } // List pro naplnìní z tabulky DB
        public Employee? Manager { get; set; } // Projektový manažer zobrazeného projektu. Tato vlastnost je zdroj pro zobrazení manažera v html.

        public List<Employee> SupportList; // List zamìstnancù podpory daného projektu. Tento List je zdroj pro zobrazení supportù v html.

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employees = _context.Employees.ToList();
            Project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Project == null)
            {
                return NotFound();
            }
            else
            {
                // Získání manažera zobrazeného projektu
                foreach (var e in Employees)
                {
                    if (e.Id == Project.ManagerId)
                    {
                        Manager = e;
                    }
                }
            }
            GetSupportList();
            Project.UpdateProjectStatus();
            
            return Page();
        }

        /// <summary>
        /// Naplní List SupportList zamìstnanci podpory daného projektu.
        /// </summary>
        private void GetSupportList()
        {
            SupportList = new List<Employee>();
            if (Project?.Support?.Count > 0) 
            {
                foreach(var i in Project.Support)
                {
                    foreach (var e in Employees)
                    {
                        if (i == e.Id)
                        {
                            SupportList.Add(e);
                        }
                    }
                }
            }
        }
    }
}
