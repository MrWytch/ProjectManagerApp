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
    /// Str�nka ProjectEdit slou�� k editaci zvolen�ho projektu.
    /// </summary>
    public class ProjectEditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProjectEditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project? Project { get; set; } // Editovan� projekt
        
        [BindProperty]
        public int? Support1Id { get; set; } // Prvn� zam�stnanec podpory

        [BindProperty]
        public int? Support2Id { get; set; } // Druh� zam�stnanec podpory

        // Select Listy pro dropdown menu v html
        public SelectList? ProjectTypeSelectList { get; set; }
        public SelectList? EmployeeSelectList { get; set; }
        public SelectList? Support1SelectList { get; set; }
        public SelectList? Support2SelectList { get; set; }
        
        // Listy pro napln�n� z tabulek DB
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

            // P��prava seznamu zam�stnanc� (Supporti projektu), aby v html v dropdown menu mohly b�t p�ednastaven� hodnoty
            if (Project.Support != null && Project.Support.Count > 0)
            {
                // Pro prvn�ho zam�stnance
                Support1Id = Project.Support.Count > 0 ? Project.Support[0] : null;

                // Pro druh�ho zam�stnance
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

            // Kontrola existence listu support�, p��padn� inicializace
            if (Project.Support == null)
            {
                Project.Support = new List<int>();
            }

            // Zaji�t�n� dostate�n� kapacity listu support�
            while (Project.Support.Count < 2)
            {
                Project.Support.Add(0);
            }

            // Nastav� hodnoty v listu support� dle hodnot vybran�ch v dropdown menu v html. Pokud nen� vybr�n ��dn� zam�stnanec, nastav� 0
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
                // Zpracov�n� ostatn�ch mo�n�ch v�jimek z datab�ze
                ModelState.AddModelError("", "Nastala chyba p�i ukl�d�n� zm�n.");
                return Page();
            }

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

            // ��st k�du pro kontrolu, zda n�kter� z upravovan�ch zam�stnanc� nen� po zm�n�ch p�et�en� (Employee.IsBusy == True)
            #region Kontrola vyt�enosti
            // List upravovan�ch zam�stnanc� u dan�ho projektu ([0] = mana�er projektu, [1] a [2] = supporti)
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
            // Pokud existuj� jm�na vyt�en�ch zam�stnanc�, napln� se TempData vyrovnou hl�kou. Ta se p�i n�sledn�m p�esm�rov�n� p�enese na str�nku /ProjectDetails a zobraz� se
            if (names != null)
            {
                TempData["Overloaded"] = "Upozorneni na vysokou vytizenost u zamestnancu " + names;
            }
            #endregion

            return RedirectToPage("./ProjectDetails", new { id = Project.Id });
        }

        /// <summary>
        /// Napln�n� list� zam�stnanc�, kter� jsou zdrojem pro dropdown menu v html. Na��taj� se pouze aktivn� zam�stnanci.
        /// </summary>
        private void LoadDropdownData()
        {
            ProjectTypeSelectList = new SelectList(Enum.GetValues(typeof(ProjectTypeEnum)));
            // Filtrujeme zam�stnance, aby v seznamu byli pouze ti, kte�� maj� IsActive nastaveno na true
            EmployeeSelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Project.ManagerId);
            Support1SelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Support1Id);
            Support2SelectList = new SelectList(_context.Employees.Where(e => e.IsActive), "Id", "Name", Support2Id);
        }
    }
}
