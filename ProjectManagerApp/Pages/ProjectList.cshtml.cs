using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Str�nka ProjectList slou�� k tabulkov�mu zobrazen� seznamu proejkt� v tabulce datab�ze. Umo��uje p�ej�t na detaily jednotliv�ch projekt� nebo p�idat nov� projekt.
    /// </summary>
    public class ProjectListModel : PageModel
    {
		private readonly ApplicationDbContext _context;

		public ProjectListModel(ApplicationDbContext context)
		{
			_context = context;
		}

		// Listy pro napln�n� z tabulek DB.
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
        /// Metoda zji��uj� jm�no mana�era dan�ho projektu.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Vrac� string jm�no mana�era</returns>
        public string GetManagerOfProject(Project p)
		{
            foreach (var e in Employees)
            {
                if (e.Id == p.ManagerId)
                {
                    return e.Name.ToString();
                }
            }
            return "Nep�i�azen";
        }

        
    }
}
