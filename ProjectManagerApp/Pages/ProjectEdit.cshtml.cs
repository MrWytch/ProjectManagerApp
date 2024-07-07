using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using ProjectManagerApp.Enums;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Stránka ProjectEdit slouží k editaci zvoleného projektu.
    /// </summary>
    public class ProjectEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectEditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project? Project { get; set; } // Editovaný projekt
        
        [BindProperty]
        public int? Support1Id { get; set; } // První zamìstnanec podpory

        [BindProperty]
        public int? Support2Id { get; set; } // Druhý zamìstnanec podpory

        // Select Listy pro dropdown menu v html
        public SelectList? ProjectTypeSelectList { get; set; }
        public SelectList? EmployeeSelectList { get; set; }
        public SelectList? Support1SelectList { get; set; }
        public SelectList? Support2SelectList { get; set; }
        
        // Listy pro naplnìní z tabulek DB
        public IList<Project>? Projects { get; set; }
        public IList<Employee>? Employees { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Project = await _context.Projects.FindAsync(id);

            if (Project == null)
            {
                return NotFound();
            }
            LoadDropdownData();

            // Pøíprava seznamu zamìstnancù (Supporti projektu), aby v html v dropdown menu mohly být pøednastavené hodnoty
            if (Project.Support != null && Project.Support.Count > 0)
            {
                // Pro prvního zamìstnance
                Support1Id = Project.Support.Count > 0 ? Project.Support[0] : null;

                // Pro druhého zamìstnance
                Support2Id = Project.Support.Count > 1 ? Project.Support[1] : null;
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData();
                return Page();
            }

            // Kontrola existence listu supportù, pøípadná inicializace
            if (Project.Support == null)
            {
                Project.Support = new List<int>();
            }

            // Zajištìní dostateèné kapacity listu supportù
            while (Project.Support.Count < 2)
            {
                Project.Support.Add(0);
            }

            // Nastaví hodnoty v listu supportù dle hodnot vybraných v dropdown menu v html. Pokud není vybrán žádný zamìstnanec, nastaví 0
            Project.Support[0] = Support1Id ?? 0;
            Project.Support[1] = Support2Id ?? 0;

            _context.Attach(Project).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Projects.Any(e => e.Id == Project.Id))
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
                // Zpracování ostatních možných výjimek z databáze
                ModelState.AddModelError("", "Nastala chyba pøi ukládání zmìn.");
                return Page();
            }

            // Èást kódu pro update vytížení všech zamìstnancù po provedených zmìnách (update hodntoy Employee.Workload a Employee.IsBusy)
            #region Update vytíženosti
            Projects = _context.Projects.ToList();
            Employees = _context.Employees.ToList();
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
            #endregion

            _context.SaveChanges(); // Znovuuložení po updatu Workload a IsBusy

            // Èást kódu pro kontrolu, zda nìkterý z upravovaných zamìstnancù není po zmìnách pøetížený (Employee.IsBusy == True)
            #region Kontrola vytíženosti
            // List upravovaných zamìstnancù u daného projektu ([0] = manažer projektu, [1] a [2] = supporti)
            List<Employee>? tempListOfEmployees =
            [
                _context.Employees.FirstOrDefault(e => e.Id == Project.ManagerId),
                _context.Employees.FirstOrDefault(e => e.Id == Project.Support[0]),
                _context.Employees.FirstOrDefault(e => e.Id == Project.Support[1]),
            ];

            string? names = null; // Sem se za sebe ukládají jména zcela vytížených zamìstnancù
            foreach (Employee e in tempListOfEmployees)
            {
                if (e != null)
                {
                    e.UpdateIsBusyStatus();
                    if (e.IsBusy)
                    {
                        names += e.Name + ", "; // Jména vytížených zamìstnancù se zapisují za sebe
                    }
                }

            }
            // Pokud existují jména vytížených zamìstnancù, naplní se TempData vyrovnou hláškou. Ta se pøi následném pøesmìrování pøenese na stránku /ProjectDetails a zobrazí se
            if (names != null)
            {
                TempData["Overloaded"] = "Upozorneni na vysokou vytizenost u zamestnancu " + names;
            }
            #endregion

            return RedirectToPage("./ProjectDetails", new { id = Project.Id });
        }

        /// <summary>
        /// Naplnìní listù zamìstnancù, které jsou zdrojem pro dropdown menu v html. Naèítají se pouze aktivní zamìstnanci.
        /// </summary>
        private void LoadDropdownData()
        {
            ProjectTypeSelectList = new SelectList(Enum.GetValues(typeof(ProjectTypeEnum)));
            // Filtrujeme zamìstnance, aby v seznamu byli pouze ti, kteøí mají IsActive nastaveno na true
            EmployeeSelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Project.ManagerId);
            Support1SelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Support1Id);
            Support2SelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Support2Id);
        }
    }
}
