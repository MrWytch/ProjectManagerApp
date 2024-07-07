using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Enums;
using System.Net.NetworkInformation;

namespace ProjectManagerApp.Models
{
    /// <summary>
    /// Třída reprezentuje zaměstnance. Slouží jako model pro DbContext.
    /// </summary>
    public class Employee
    {
        public int Id { get; set; } // Primární klíč
        public string Name { get; set; } // Jmén oa příjmení zaměstnance
        public string? Email { get; set; } // E-mail zaměstnance
        public string? Phone { get; set; } // telefonní číslo zaměstnance
        public int Workload { get; set; } // Hodnota vytížení zaměstnance (0 - 100%). Manažer projektu +50%, podpora projektu +25%
        public bool IsBusy { get; set; } // Zaměstnanec má Workload >= 100
        public bool IsActive { get; set; } // Záznam zaměstnance je aktivní

        /// <summary>
        /// Metoda updatuje vlastnost IsBusy na základě vlastnosti Workload.
        /// </summary>
        public void UpdateIsBusyStatus()
        {
            if (Workload < 100)
            {
                IsBusy = false;
            }
            else
            {
                IsBusy = true;
            }
        }

        /// <summary>
        /// Metoda přetváří binární hodnotu vlastnosti IsBusy na srozumitelný text.
        /// </summary>
        /// <returns>Vrací textový řetězec "Volný" nebo "Zaneprázdněn"</returns>
        public string GetIsBusyStatusString()
        {
            if (IsBusy) 
            {
                return "Zaneprázdněn";
            }else
            {
                return "Volný";
            }
        }

        /// <summary>
        /// Metoda určuje barvu pro zobrazení v html na základě vytíženosti zaměstnance. 
        /// </summary>
        /// <returns>Vrací textový řetězec určující barvu pro použití v html bootstrap stylech</returns>
        public string GetIsBusyStatusColor()
        {
            if (IsBusy) 
            {
                return "bg-danger text-white";
            }else
            {
                return "bg-success text-white";
            }
        }

        /// <summary>
        /// Metoda přetváří binární hodnotu vlastnosti IsActive na srozumitelný text.
        /// </summary>
        /// <returns>Vrací textový řetězec "Aktivní" nebo "Neaktivní"</returns>
        public string GetIsActiveStatusString()
        {
            if (IsActive)
            {
                return "Aktivní";
            }
            else
            {
                return "Neaktivní";
            }
        }

        /// <summary>
        /// Metoda určuje barvu pro zobrazení v html na základě toho, zda je záznam zaměstnance aktivní.
        /// </summary>
        /// <returns>Vrací textový řetězec určující barvu pro použití v html bootstrap stylech</returns>
        public string GetIsActiveStatusColor()
        {
            if (IsActive)
            {
                return "bg-success text-white";
            }
            else
            {
                return "bg-dark text-white";
            }
        }
    }

    
}
