using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using ProjectManagerApp.Enums;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Stránka ProjectAdd slouží k pøidání nového projektu vè. jeho uložení do DB.
    /// </summary>
    public class ProjectAddModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectAddModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project Project { get; set; } = new Project(); // Pøidávaný projekt

        [BindProperty]
        public int? Support1Id { get; set; } // První zamìstnanec podpory pøidávaného projektu

        [BindProperty]
        public int? Support2Id { get; set; } // Druhý zamìstnanec podpory pøidávaného projektu

        // Select Listy pro dropdown menu v html formuláøi
        public SelectList? ProjectTypeSelectList { get; set; }
        public SelectList? EmployeeSelectList { get; set; }
        public SelectList? Support1SelectList { get; set; }
        public SelectList? Support2SelectList { get; set; }

        // Listy pro naplnìní z tabulek DB
        public IList<Project>? Projects { get; set; }
        public IList<Employee>? Employees { get; set; }

        public void OnGet()
        {
            LoadDropdownData();

            // Èást kódu, která automaticky pøiøadí projektu jeho èíslo (Project.Number) dle nejbližšího volného v DB tabulce Projects
            #region Pøiøazení èísla projektu
            var highestId = _context.Projects.Max(p => (int?)p.Id) ?? 0; // Najde nejvyšší ID projektu nebo vrátí 0, pokud neexistuje žádný záznam
            int newId = highestId + 1; // Èíslo nového projektu je o 1 vìtší
            
            // Tvorba èísla projektu v požadovaném formátu PROJ-####
            if (newId < 10)
            {
                Project.Number = $"PROJ-000{newId}"; 
            }
            else if (newId < 100) 
            {
                Project.Number = $"PROJ-00{newId}";
            }
            else if (newId < 1000)
            {
                Project.Number = $"PROJ-0{newId}";
            }
            else
            {
                Project.Number = $"PROJ-{newId}";
            }
            #endregion
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData();
                return Page();
            }

            // Vytvoøení Listu pro zamìstnance podpory nového projektu
            Project.Support = new List<int>();
            // Zajištìní dostateèné kapacity listu (2)
            while (Project.Support.Count < 2)
            {
                Project.Support.Add(0);
            }
            //Naplnìní Listu supportù dle hodnot vybraných v html formuláøi.
            Project.Support[0] = Support1Id ?? 0;
            Project.Support[1] = Support2Id ?? 0;

            _context.Projects.Add(Project);
            _context.SaveChanges();

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

            // Èást kódu pro kontrolu, zda nìkterý ze zamìstnancù není po pøidání projektu zcela vytížený (Employee.IsBusy == True)
            #region Kontrola vytíženosti
            // List zamìstnancù u pøidaného projektu ([0] = manažer projektu, [1] a [2] = supporti)
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
            // Pokud existují jména vytížených zamìstnancù, naplní se TempData vyrovnou hláškou. Ta se pøi následném pøesmìrování pøenese na stránku /ProjectList a zobrazí se
            if (names != null)
            {
                TempData["Overloaded"] = "Upozorneni na vysokou vytizenost u zamestnancu " + names;
            }
            #endregion

            TempData["ProjectAdded"] = "Zaznam byl uspesne pridan."; // Pro js alert o úspìchu na stránce Projectlist

            return RedirectToPage("./ProjectList"); // Pøesmìrování na seznam projektù po úspìšném pøidání
        }

        /// <summary>
        /// Naplnìní listù zamìstnancù, které jsou zdrojem pro dropdown menu v html formuláøi. Naèítají se pouze aktivní zamìstnanci.
        /// </summary>
        private void LoadDropdownData()
        {
            ProjectTypeSelectList = new SelectList(Enum.GetValues(typeof(ProjectTypeEnum)));
            // Filtrujeme zamìstnance, aby v seznamu byli pouze ti, kteøí mají IsActive nastaveno na true
            EmployeeSelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name");
            Support1SelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Support1Id);
            Support2SelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Support2Id);
        }
    }
}
