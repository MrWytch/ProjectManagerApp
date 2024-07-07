using Microsoft.IdentityModel.Tokens;
using ProjectManagerApp.Enums;
using System.Diagnostics.Eventing.Reader;

namespace ProjectManagerApp.Models
{
    /// <summary>
    /// Třída reprezentuje projekt. Slouží jako model pro DbContext.
    /// </summary>
    public class Project
    {
        public int Id { get; set; } // Primární klíč
        public string Number { get; set; } // označení projektu ve formátu "PROJ-####"
        public string? Name { get; set; } // Název projektu
        public ProjectTypeEnum ProjectType { get; set; } // Typ projektu (výčtový typ)
        public DateTime? StartDate { get; set; } // Datum začátku
        public DateTime? FinishDate { get; set; } // Datum ukončení
        public int? ManagerId { get; set; } // ID vedoucího projektu
        public List<int>? Support { get; set; } // List IDček supportů
        public int Progress { get; set; } // Pokrok v projektu 0 - 100 %
        public ProjectStatusEnum Status { get; set; } // Status projektu (výčtový typ)
        public bool CostDownGoal { get; set; } // Projekt redukuje náklady
        public bool SafetyGoal { get; set; } // Projekt zvyšuje bezpečnost
        public bool ProductivityGoal { get; set; } // Projekt zvyšuje produktivitu
        public bool EnvironmentGoal { get; set; } // Prolekt má pozitivní efekt na ŽP
        public bool WorkabilityGoal { get; set; } // Projekt umožňuje vykonávat proces
        public bool WorkComfortGoal { get; set; } // Projekt zlepšuje komfort při práci
        public string? Description { get; set; } // Textový popis projektu
        public string? ActualState { get; set; } // Textové zhodnocení aktuálního stavu projektu


        /// <summary>
        /// Metoda počítá teoretický pokrok projektu (0 - 100%), který by měl projekt v současnosti mít (Datum začátku = 0%, Datum ukončení = 100%)
        /// </summary>
        /// <returns>Vrací teoretický aktuální pokrok, int 0 - 100</returns>
        private int GetVirtualProgress()
        {
            int vProgress = 0;

            if (StartDate != null && FinishDate != null)
            {
                TimeSpan totalDuration = FinishDate.Value - StartDate.Value;
                int totalDays = (int)totalDuration.TotalDays;
                TimeSpan elapsedDuration = DateTime.Now - StartDate.Value;
                int elapsedDays = (int)elapsedDuration.TotalDays;
                // ratio = ;
                vProgress = (int)(((double)elapsedDays / (double)totalDays) * 100);
            }
            return vProgress;
            
        }
        /// <summary>
        /// Metoda updatuje status projektu.
        /// </summary>
        public void UpdateProjectStatus()
        {
            if (Progress >= 100)
            {
                Status = ProjectStatusEnum.Finished;
            }
            else if (DateTime.Now < StartDate)
            {
                Status = ProjectStatusEnum.NotStarted;
            }
            else if (Progress < GetVirtualProgress())
            {
                Status = ProjectStatusEnum.Delayed;    
            }
            else
            {
                Status = ProjectStatusEnum.OnSchedule;
            }

        }

        /// <summary>
        /// Metoda přetváří hodnoty výčtového typu Status na srozumitelnější text.
        /// </summary>
        /// <returns>Vrací textový řetězec</returns>
        public string GetProjectStatusString()
        {
            switch (Status)
            {
                case ProjectStatusEnum.Undefined:
                    return "Neznámý";
                    
                case ProjectStatusEnum.NotStarted:
                    return "Nezačal";
                    
                case ProjectStatusEnum.OnSchedule:
                    return "Dle plánu";
                    
                case ProjectStatusEnum.Delayed:
                    return "Zpožděn";
                    
                case ProjectStatusEnum.Finished:
                    return "Dokončen";
                    
                default:
                    return "Neznámý";
            }
        }

        /// <summary>
        /// Metoda přetváří hodnoty výčtového typu ProjectType na srozumitelnější text.
        /// </summary>
        /// <returns>>Vrací textový řetězec</returns>
        public string GetProjectTypeString()
        {
            switch (ProjectType)
            {
                case ProjectTypeEnum.Neurceno:
                    return "Neurčeno";

                case ProjectTypeEnum.ReLayout:
                    return "Re-layout";

                case ProjectTypeEnum.ZmenaProcedury:
                    return "Změna procedury";

                case ProjectTypeEnum.NovyProdukt:
                    return "Nový produkt";

                case ProjectTypeEnum.NovaVyrobniLinka:
                    return "Nová výrobní linka";

                case ProjectTypeEnum.ZmenaTechnologie:
                    return "Změna technologie";

                default:
                    return "Neurčeno";
            }
        }

        /// <summary>
        /// Metoda určuje barvu pro zobrazení v html na základě stavu projektu.
        /// </summary>
        /// <returns>Vrací textový řetězec určující barvu pro použití v html bootstrap stylech</returns>
        public string GetProjectStatusColor()
        {
            switch (Status)
            {
                case ProjectStatusEnum.Undefined:
                    return "bg-light text-dark";
                    
                case ProjectStatusEnum.NotStarted:
                    return "bg-info text-dark";
                    
                case ProjectStatusEnum.OnSchedule:
                    return "bg-success text-white";
                    
                case ProjectStatusEnum.Delayed:
                    return "bg-danger text-white";
                    
                case ProjectStatusEnum.Finished:
                    return "bg-dark text-white";
                    
                default:
                    return "bg-light text-dark";
            }
        }
    }
}
