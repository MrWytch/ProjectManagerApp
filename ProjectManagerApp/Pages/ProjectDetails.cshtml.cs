using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Str�nka ProjectDetails slou�� k zobrazen� detail� zvolen�ho projektu (karta projektu).
    /// </summary>
    public class ProjectDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Project? Project { get; set; } // Projekt, jeho� detaily se maj� zobrazit
        public IList<Employee> Employees { get; set; } // List pro napln�n� z tabulky DB
        public Employee? Manager { get; set; } // Projektov� mana�er zobrazen�ho projektu. Tato vlastnost je zdroj pro zobrazen� mana�era v html.

        public List<Employee> SupportList; // List zam�stnanc� podpory dan�ho projektu. Tento List je zdroj pro zobrazen� support� v html.

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
                // Z�sk�n� mana�era zobrazen�ho projektu
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
        /// Napln� List SupportList zam�stnanci podpory dan�ho projektu.
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
