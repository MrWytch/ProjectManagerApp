# ProjectManagerApp

Webová aplikace pro správu projektů. Určeno pouze pro výuku.
Aplikace pracuje s daty uloženými v lokální SQL databázi.
Frameworky: ASP.NET Core + Entity Framework Core + Razor Pages

Před prvním spuštením nejprve nutné spustit migraci, která vytvoří databázi s tabulkami. Po prvním spuštění je na úvodní stránce možnost nahrát do databáze testovací data, se kterými lze dále pracovat.

Aplikace umožňuje uživateli
- zobrazit seznam projektů se základními vlastnostmi
- zobrazit detaily vybraného projektu
- editovat vybraný projekt
- zobrazit seznam zaměstnanců
- zobrazit detaily vybraného zaměstnance
- editovat vybraného zaměstnance

Aplikace se mj. stará o 
- aktualizaci některých vlastností na základě uživatelských změn (např. vytíženost zaměstnanců nebo status projektu)
- barevnou vizualizaci některých vlastností na základě jejich hodnot
- upozornění uživatele na některé situace pomocí popup oken (např. vysoká vytíženost zaměstnanců)
- validaci některých vstupů uživatele, přičemž nežádoucí hodnoty jsou do značné míry eliminované zejm. vhodnou architekturou formulářů bez nutnosti implementovat validační metody
