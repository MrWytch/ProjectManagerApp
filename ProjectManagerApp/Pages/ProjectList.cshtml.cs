using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Stránka ProjectList slouí k tabulkovému zobrazení seznamu proejktù v tabulce databáze. Umoòuje pøejít na detaily jednotlivıch projektù nebo pøidat novı projekt.
    /// </summary>
    public class ProjectListModel : PageModel
    {
		private readonly ApplicationDbContext _context;

		public ProjectListModel(ApplicationDbContext context)
		{
			_context = context;
		}

		// Listy pro naplnìní z tabulek DB.
        public IList<Project>? Projects { get; set; }
        public IList<Employee>? Employees { get; set; }

        public void OnGet()
        {
            Projects = _context.Projects.ToList();
            Employees = _context.Employees.ToList();
            foreach (var p in Projects)
            {
                p.UpdateProjectStatus();
            }
            _context.SaveChanges();
        }

		/// <summary>
        /// Metoda zjišujì jméno manaera daného projektu.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Vrací string jméno manaera</returns>
        public string GetManagerOfProject(Project p)
		{
            foreach (var e in Employees)
            {
                if (e.Id == p.ManagerId)
                {
                    return e.Name.ToString();
                }
            }
            return "Nepøiøazen";
        }

        
    }
}
