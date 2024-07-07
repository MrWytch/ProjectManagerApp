using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Str�nka HumanResources slou�� pro zobrazen� seznamu zam�stnanc� v datab�zi. Umo��uje p�ej�t na kartu ka�d�ho zam�stnance nebo p�idat nov�ho.
    /// </summary>
    public class HumanResourcesModel : PageModel
    {
		private readonly ApplicationDbContext _context;

		public HumanResourcesModel(ApplicationDbContext context)
		{
			_context = context;
		}

		// Listy pro napln�n� z tabulek DB.
        public IList<Employee> Employees { get; set; }
        public IList<Project> Projects { get; set; }

        /// <summary>
        /// P�i na�ten� str�nky se napln� Listy, kter� slou�� jako zdroj dat pro tabulku v html. Z�rove� se tak� updatuje vyt�enost jednotliv�ch zam�stnanc� (Employee.Workload a Employee.IsBusy)
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
                    // Zam�stnance nevyt�uj� projekty ve stavu NotStarted ani Finished
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
