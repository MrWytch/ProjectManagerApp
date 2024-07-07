using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using ProjectManagerApp.Enums;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Str�nka ProjectAdd slou�� k p�id�n� nov�ho projektu v�. jeho ulo�en� do DB.
    /// </summary>
    public class ProjectAddModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectAddModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project Project { get; set; } = new Project(); // P�id�van� projekt

        [BindProperty]
        public int? Support1Id { get; set; } // Prvn� zam�stnanec podpory p�id�van�ho projektu

        [BindProperty]
        public int? Support2Id { get; set; } // Druh� zam�stnanec podpory p�id�van�ho projektu

        // Select Listy pro dropdown menu v html formul��i
        public SelectList? ProjectTypeSelectList { get; set; }
        public SelectList? EmployeeSelectList { get; set; }
        public SelectList? Support1SelectList { get; set; }
        public SelectList? Support2SelectList { get; set; }

        // Listy pro napln�n� z tabulek DB
        public IList<Project>? Projects { get; set; }
        public IList<Employee>? Employees { get; set; }

        public void OnGet()
        {
            LoadDropdownData();

            // ��st k�du, kter� automaticky p�i�ad� projektu jeho ��slo (Project.Number) dle nejbli���ho voln�ho v DB tabulce Projects
            #region P�i�azen� ��sla projektu
            var highestId = _context.Projects.Max(p => (int?)p.Id) ?? 0; // Najde nejvy��� ID projektu nebo vr�t� 0, pokud neexistuje ��dn� z�znam
            int newId = highestId + 1; // ��slo nov�ho projektu je o 1 v�t��
            
            // Tvorba ��sla projektu v po�adovan�m form�tu PROJ-####
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

            // Vytvo�en� Listu pro zam�stnance podpory nov�ho projektu
            Project.Support = new List<int>();
            // Zaji�t�n� dostate�n� kapacity listu (2)
            while (Project.Support.Count < 2)
            {
                Project.Support.Add(0);
            }
            //Napln�n� Listu support� dle hodnot vybran�ch v html formul��i.
            Project.Support[0] = Support1Id ?? 0;
            Project.Support[1] = Support2Id ?? 0;

            _context.Projects.Add(Project);
            _context.SaveChanges();

            // ��st k�du pro update vyt�en� v�ech zam�stnanc� po proveden�ch zm�n�ch (update hodntoy Employee.Workload a Employee.IsBusy)
            #region Update vyt�enosti
            Projects = _context.Projects.ToList();
            Employees = _context.Employees.ToList();
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
            #endregion

            _context.SaveChanges(); // Znovuulo�en� po updatu Workload a IsBusy

            // ��st k�du pro kontrolu, zda n�kter� ze zam�stnanc� nen� po p�id�n� projektu zcela vyt�en� (Employee.IsBusy == True)
            #region Kontrola vyt�enosti
            // List zam�stnanc� u p�idan�ho projektu ([0] = mana�er projektu, [1] a [2] = supporti)
            List<Employee>? tempListOfEmployees =
            [
                _context.Employees.FirstOrDefault(e => e.Id == Project.ManagerId),
                _context.Employees.FirstOrDefault(e => e.Id == Project.Support[0]),
                _context.Employees.FirstOrDefault(e => e.Id == Project.Support[1]),
            ];

            string? names = null; // Sem se za sebe ukl�daj� jm�na zcela vyt�en�ch zam�stnanc�
            foreach (Employee e in tempListOfEmployees) 
            {
                if (e != null)
                {
                    e.UpdateIsBusyStatus();
                    if (e.IsBusy)
                    {
                        names += e.Name + ", "; // Jm�na vyt�en�ch zam�stnanc� se zapisuj� za sebe
                    }
                }
            }
            // Pokud existuj� jm�na vyt�en�ch zam�stnanc�, napln� se TempData vyrovnou hl�kou. Ta se p�i n�sledn�m p�esm�rov�n� p�enese na str�nku /ProjectList a zobraz� se
            if (names != null)
            {
                TempData["Overloaded"] = "Upozorneni na vysokou vytizenost u zamestnancu " + names;
            }
            #endregion

            TempData["ProjectAdded"] = "Zaznam byl uspesne pridan."; // Pro js alert o �sp�chu na str�nce Projectlist

            return RedirectToPage("./ProjectList"); // P�esm�rov�n� na seznam projekt� po �sp�n�m p�id�n�
        }

        /// <summary>
        /// Napln�n� list� zam�stnanc�, kter� jsou zdrojem pro dropdown menu v html formul��i. Na��taj� se pouze aktivn� zam�stnanci.
        /// </summary>
        private void LoadDropdownData()
        {
            ProjectTypeSelectList = new SelectList(Enum.GetValues(typeof(ProjectTypeEnum)));
            // Filtrujeme zam�stnance, aby v seznamu byli pouze ti, kte�� maj� IsActive nastaveno na true
            EmployeeSelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name");
            Support1SelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Support1Id);
            Support2SelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Support2Id);
        }
    }
}
