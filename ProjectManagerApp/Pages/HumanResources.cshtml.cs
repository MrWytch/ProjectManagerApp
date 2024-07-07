using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Stránka HumanResources slouží pro zobrazení seznamu zamìstnancù v databázi. Umožòuje pøejít na kartu každého zamìstnance nebo pøidat nového.
    /// </summary>
    public class HumanResourcesModel : PageModel
    {
		private readonly ApplicationDbContext _context;

		public HumanResourcesModel(ApplicationDbContext context)
		{
			_context = context;
		}

		// Listy pro naplnìní z tabulek DB.
        public IList<Employee> Employees { get; set; }
        public IList<Project> Projects { get; set; }

        /// <summary>
        /// Pøi naètení stránky se naplní Listy, které slouží jako zdroj dat pro tabulku v html. Zároveò se také updatuje vytíženost jednotlivých zamìstnancù (Employee.Workload a Employee.IsBusy)
        /// </summary>
        public void OnGet()
        {
			Employees = _context.Employees.ToList();
            Projects = _context.Projects.ToList();
            foreach (var e in Employees)
            {
                e.Workload = 0;
                foreach (var p in Projects)
                {
                    // Zamìstnance nevytìžují projekty ve stavu NotStarted ani Finished
                    if ((p.Status != Enums.ProjectStatusEnum.NotStarted) && (p.Status != Enums.ProjectStatusEnum.Finished))
                    {
                        if (p.ManagerId == e.Id)
                        {
                            e.Workload += 50;
                        }
                        if (p.Support.Contains(e.Id))
                        {
                            e.Workload += 25;
                        }
                    }
                    
                }
                e.UpdateIsBusyStatus();
            }
            _context.SaveChanges();
        }
    }
}
