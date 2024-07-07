using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Models;
using ProjectManagerApp.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var dbContext = services.GetRequiredService<ApplicationDbContext>();


//    var project = new Project
//    {
//        Number = "PROJ-003",
//        Name = "Projekt 3",
//        ProjectType = ProjectTypeEnum.NovaVyrobniLinka,
//        StartDate = DateTime.Now,
//        FinishDate = DateTime.Now.AddDays(60),
//        ManagerId = 2,
//        Progress = 20,
//        Status = ProjectStatusEnum.Undefined,
//        CostDownGoal = true,
//        SafetyGoal = false,
//        ProductivityGoal = false,
//        EnvironmentGoal = true,
//        WorkabilityGoal = true,
//        WorkComfortGoal = false,
//        Description = "Popis projektu 3",
//        ActualState = "Popis aktuálního stavu projektu 3"
//    };

//    var manager = new Employee
//    {
//        Name = "Karel Sýkora",
//        Email = "sykora.k@spolecnost.cz",
//        Phone = "+420 777 369 258",
//        Workload = 50,
//        IsBusy = false,
//        IsActive = true
//    };

//    dbContext.Projects.Add(project);
//    dbContext.Employees.Add(manager);
//    dbContext.SaveChanges();

//    Console.WriteLine("Testovací projekt byl úspìšnì vytvoøen!");
//    Console.ReadKey();
//}

app.Run();
