using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagerApp.Models;  // Pøidat pøístup k datovým modelùm

namespace ProjectManagerApp.Pages
{
    /// <summary>
    /// Stránka Index slouží jako uvítací. Zároveò pøi naètení zkontroluje, zda existují záznamy v DB tabulkách Projects a Employees. Pokud ano, nic se nestane. Pokud ne, nabídne možnost vytvoøit testovací data.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public bool IsDatabaseEmpty { get; set; }  // Vlastnost pro zjištìní stavu databáze

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            IsDatabaseEmpty = !_context.Projects.Any() && !_context.Employees.Any(); // Zjištìní existencá záznamù v tabulkách
        }

        /// <summary>
        /// Metoda naplní databázi prvotními daty. 
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostInitializeDatabase()
        {
            // Metoda pro inicializaci databáze s prvotními daty
            if (!_context.Projects.Any() && !_context.Employees.Any())
            {
                // Pøidání projektù
                _context.Projects.Add(new Project { Number = "PROJ-0001", Name = "Název projektu 1", ProjectType = Enums.ProjectTypeEnum.ZmenaTechnologie, StartDate = Convert.ToDateTime("01.06.2024 00:00:00"), FinishDate = Convert.ToDateTime("26.07.2024 00:00:00"), ManagerId = 1, Support = new List<int> { 3, 2 }, Progress = 100, CostDownGoal = true, SafetyGoal = true, ProductivityGoal = true, EnvironmentGoal = true, Description = "Popis projektu 1", ActualState = "Aktuální stav projektu 1" });
                _context.Projects.Add(new Project { Number = "PROJ-0002", Name = "Název projektu 2", ProjectType = Enums.ProjectTypeEnum.Neurceno, StartDate = Convert.ToDateTime("01.07.2024 00:00:00"), FinishDate = Convert.ToDateTime("18.08.2024 00:00:00"), ManagerId = 5, Support = new List<int> { 2, 7 }, Progress = 45, WorkabilityGoal = true, Description = "Popis projektu 2", ActualState = "Aktuální stav projektu 2" });
                _context.Projects.Add(new Project { Number = "PROJ-0003", Name = "Název projektu 3", ProjectType = Enums.ProjectTypeEnum.ZmenaProcedury, StartDate = Convert.ToDateTime("01.01.2024 00:00:00"), FinishDate = Convert.ToDateTime("31.12.2024 00:00:00"), ManagerId = 3, Support = new List<int> { 1, 0 }, Progress = 20, CostDownGoal = true, EnvironmentGoal = true, WorkabilityGoal = true, Description = "Popis projektu 3", ActualState = "Aktuální stav projektu 3" });
                _context.Projects.Add(new Project { Number = "PROJ-0004", Name = "Název projektu 4", ProjectType = Enums.ProjectTypeEnum.Neurceno, StartDate = Convert.ToDateTime("01.08.2024 00:00:00"), FinishDate = Convert.ToDateTime("31.10.2025 00:00:00"), ManagerId = 2, Support = new List<int> { 7, 3 }, Progress = 0, CostDownGoal = true, SafetyGoal = true, WorkabilityGoal = true, Description = "Popis projektu 4", ActualState = "Aktuální stav projektu 4" });
                _context.Projects.Add(new Project { Number = "PROJ-0005", Name = "Název projektu 5", ProjectType = Enums.ProjectTypeEnum.NovyProdukt, StartDate = Convert.ToDateTime("01.10.2024 00:00:00"), FinishDate = Convert.ToDateTime("28.02.2025 00:00:00"), ManagerId = 7, Support = new List<int> { 3, 0 }, Progress = 0, CostDownGoal = true, ProductivityGoal = true, WorkabilityGoal = true, Description = "Popis projektu 5", ActualState = "Aktuální stav projektu 5" });
                _context.Projects.Add(new Project { Number = "PROJ-0006", Name = "Název projektu 6", ProjectType = Enums.ProjectTypeEnum.Neurceno, StartDate = Convert.ToDateTime("01.06.2024 00:00:00"), FinishDate = Convert.ToDateTime("30.11.2024 00:00:00"), ManagerId = 7, Support = new List<int> { 3, 0 }, Progress = 80, SafetyGoal = true, WorkComfortGoal = true, Description = "Popis projektu 6", ActualState = "Aktuální stav projektu 6" });
                _context.Projects.Add(new Project { Number = "PROJ-0007", Name = "Název projektu 7", ProjectType = Enums.ProjectTypeEnum.NovyProdukt, StartDate = Convert.ToDateTime("01.02.2024 00:00:00"), FinishDate = Convert.ToDateTime("31.08.2024 00:00:00"), ManagerId = 7, Support = new List<int> { 3, 0 }, Progress = 25, CostDownGoal = true, EnvironmentGoal = true, Description = "Popis projektu 7", ActualState = "Aktuální stav projektu 7" });

                // Pøidání zamìstnancù
                _context.Employees.Add(new Employee { Name = "Václav Hrdlièka", Email = "hrdlicka.v@spolecnost.cz", Phone = "+420 777 369 258", IsActive = true });
                _context.Employees.Add(new Employee { Name = "Jan Slavík", Email = "slavik.j@spolecnost.cz", Phone = "+420 778 379 164", IsActive = true });
                _context.Employees.Add(new Employee { Name = "Karel Sýkora", Email = "sykora.k@spolecnost.cz", Phone = "+420 608 992 333", IsActive = true });
                _context.Employees.Add(new Employee { Name = "Jiøí Èáp", Email = "cap.j@spolecnost.cz", Phone = "+420 606 069 341", IsActive = false });
                _context.Employees.Add(new Employee { Name = "Metodìj Holub", Email = "holub.m@spolecnost.cz", Phone = "+420 607 157 481", IsActive = true });
                _context.Employees.Add(new Employee { Name = "Martin Sova", Email = "sova.m@spolecnost.cz", Phone = "+420 702 458 992", IsActive = false });
                _context.Employees.Add(new Employee { Name = "Lukáš Vrána", Email = "vrana.l@spolecnost.cz", Phone = "+420 737 747 757", IsActive = true });

                _context.SaveChanges();
            }

            return RedirectToPage();  // Pøesmìrování zpìt na stránku Index
        }
    }
}
