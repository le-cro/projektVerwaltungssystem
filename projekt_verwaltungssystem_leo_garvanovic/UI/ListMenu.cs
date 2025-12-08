using projekt_verwaltungssystem_leo_garvanovic.Models;
using projekt_verwaltungssystem_leo_garvanovic.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    public static class ListMenu
    {
        public static void ShowListenMenu(Benutzer user)
        {
            try
            {
                if (!MenuContext.DataLoaded && Directory.Exists(MenuContext.DataDirectory))
                {
                    MenuContext.Data.LoadAll(MenuContext.DataDirectory, MenuContext.CsvDelimiter, clearExisting: true);
                    MenuContext.DataLoaded = true;
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error("Automatic list load failed", ex);
            }

            bool running = true;

            while (running)
            {
                UIFormatter.ClearAndHeader("Listenverwaltung");
                UIFormatter.PrintOptions(
                    "1. Mitarbeiter",
                    "2. Computer",
                    "3. Handys",
                    "4. Autos",
                    "0. Zurück"
                );

                string choice = UIFormatter.ReadOption();

                switch (choice)
                {
                    case "1":
                        ManageList("Mitarbeiter", user);
                        break;

                    case "2":
                        ManageList("Computer", user);
                        break;

                    case "3":
                        ManageList("Handys", user);
                        break;

                    case "4":
                        ManageList("Autos", user);
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        UIFormatter.Error("Ungültige Eingabe!");
                        UIFormatter.PressAnyKey();
                        break;
                }
            }
        }

        private static void ManageList(string listName, Benutzer user)
        {
            bool back = false;

            while (!back)
            {
                UIFormatter.ClearAndHeader($"{listName}-Verwaltung");
                var options = new List<string> { "1. Anzeigen" };
                if (user.Rolle == "Admin")
                {
                    options.Add("2. Hinzufügen");
                    options.Add("3. Bearbeiten");
                    options.Add("4. Löschen");
                }
                options.Add("0. Zurück");

                UIFormatter.PrintOptions(options.ToArray());

                string choice = UIFormatter.ReadOption();

                switch (choice)
                {
                    case "1":
                        ShowBrowseMenu(listName);
                        break;
                    case "2":
                        if (user.Rolle == "Admin") AddItem(listName);
                        else UIFormatter.Error("Keine Berechtigung!");
                        ConsoleUtils.Pause();
                        break;
                    case "3":
                        if (user.Rolle == "Admin") EditItem(listName);
                        else UIFormatter.Error("Keine Berechtigung!");
                        ConsoleUtils.Pause();
                        break;
                    case "4":
                        if (user.Rolle == "Admin") DeleteItem(listName);
                        else UIFormatter.Error("Keine Berechtigung!");
                        ConsoleUtils.Pause();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        UIFormatter.Error("Ungültige Eingabe!");
                        ConsoleUtils.Pause();
                        break;
                }
            }
        }

        private static void ShowBrowseMenu(string listName)
        {
            bool back = false;
            while (!back)
            {
                UIFormatter.ClearAndHeader($"{listName}: Anzeigen");
                UIFormatter.PrintOptions(
                    "1. Alle Einträge anzeigen",
                    "2. Suchen (Freitext über alle Eigenschaften)",
                    "3. Filtern (Eigenschaft + Operator)",
                    "0. Zurück"
                );

                string choice = UIFormatter.ReadOption();

                switch (choice)
                {
                    case "1":
                        ShowItems(listName);
                        ConsoleUtils.Pause();
                        break;

                    case "2":
                        SearchItems(listName);
                        ConsoleUtils.Pause();
                        break;

                    case "3":
                        FilterItems(listName);
                        ConsoleUtils.Pause();
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        UIFormatter.Error("Ungültige Eingabe!");
                        ConsoleUtils.Pause();
                        break;
                }
            }
        }

        private static void ShowItems(string listName)
        {
            var list = MenuContext.GetIList(listName);
            UIFormatter.ShowStatus($"{listName} anzeigen:");

            if (list.Count == 0)
            {
                UIFormatter.ShowStatus("Keine Einträge vorhanden.");
                return;
            }

            int i = 1;
            foreach (var item in list)
                Console.WriteLine($"  {i++}. {item}");
        }

        private static void SearchItems(string listName)
        {
            var list = MenuContext.GetIList(listName);
            Console.Write("Suchbegriff eingeben: ");
            string term = Console.ReadLine() ?? string.Empty;

            var results = list.Cast<object>()
                              .Where(o => (o?.ToString() ?? "")
                                  .IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                              .ToList();

            PrintResults(results, listName);
        }

        private static void PrintResults(IEnumerable<object> results, string listName)
        {
            Console.WriteLine($"\nTreffer in {listName}:");
            if (!results.Any())
            {
                Console.WriteLine("Keine Treffer.");
                return;
            }

            int i = 1;
            foreach (var item in results)
                Console.WriteLine($"{i++}. {item}");
        }

        private static void FilterItems(string listName)
        {
            var list = MenuContext.GetIList(listName);
            var type = MenuContext.GetItemType(listName);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanRead).ToArray();

            if (props.Length == 0)
            {
                UIFormatter.ShowStatus("Keine filterbaren Eigenschaften gefunden.");
                return;
            }

            UIFormatter.ShowStatus("Verfügbare Eigenschaften:");
            for (int i = 0; i < props.Length; i++)
                Console.WriteLine($"  {i + 1}. {props[i].Name} ({props[i].PropertyType.Name})");

            Console.Write("Eigenschaft auswählen (Nummer): ");
            if (!int.TryParse(Console.ReadLine(), out int pIndex) || pIndex < 1 || pIndex > props.Length)
            {
                UIFormatter.Error("Ungültige Auswahl.");
                return;
            }

            var prop = props[pIndex - 1];
            var all = list.Cast<object>();

            Func<object, bool>? predicate = null;

            var pt = prop.PropertyType;
            if (pt == typeof(string))
            {
                UIFormatter.ShowStatus("1. Enthält");
                UIFormatter.ShowStatus("2. Gleich");
                Console.Write("Operator: ");
                var op = Console.ReadLine();
                Console.Write("Wert: ");
                var value = Console.ReadLine() ?? string.Empty;
                if (op == "2")
                    predicate = o => string.Equals((prop.GetValue(o) as string) ?? string.Empty, value, StringComparison.OrdinalIgnoreCase);
                else
                    predicate = o => ((prop.GetValue(o) as string) ?? string.Empty).IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            else if (pt == typeof(int) || pt == typeof(double) || pt == typeof(decimal))
            {
                UIFormatter.ShowStatus("1. Gleich");
                UIFormatter.ShowStatus("2. Größer als");
                UIFormatter.ShowStatus("3. Kleiner als");
                Console.Write("Operator: ");
                var op = Console.ReadLine();
                Console.Write("Wert: ");
                var valText = Console.ReadLine() ?? string.Empty;
                if (!double.TryParse(valText, out double cmp))
                {
                    UIFormatter.Error("Ungültiger Wert.");
                    return;
                }

                predicate = o =>
                {
                    var v = prop.GetValue(o);
                    if (v == null) return false;
                    if (!double.TryParse(Convert.ToString(v), out double actual)) return false;
                    return op switch
                    {
                        "2" => actual > cmp,
                        "3" => actual < cmp,
                        _ => Math.Abs(actual - cmp) < 0.0000001
                    };
                };
            }
            else if (pt == typeof(bool))
            {
                Console.Write("Wert (j/n): ");
                var txt = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
                bool want = txt is "j" or "ja" or "y" or "yes" or "true";
                predicate = o => Convert.ToBoolean(prop.GetValue(o)) == want;
            }
            else
            {
                Console.Write("Wert (Gleichheit überprüft via ToString): ");
                var val = Console.ReadLine() ?? string.Empty;
                predicate = o => string.Equals(Convert.ToString(prop.GetValue(o)), val, StringComparison.OrdinalIgnoreCase);
            }

            var results = predicate is null ? Enumerable.Empty<object>() : all.Where(predicate).ToList();
            PrintResults(results, listName);
        }

        private static void AddItem(string listName)
        {
            switch (listName)
            {
                case "Mitarbeiter":
                    Console.Write("ID: ");
                    int pid = int.TryParse(Console.ReadLine(), out pid) ? pid : 0;
                    Console.Write("Name: ");
                    string pname = Console.ReadLine();
                    Console.Write("Alter: ");
                    int palter = int.TryParse(Console.ReadLine(), out palter) ? palter : 0;
                    Console.Write("Abteilung: ");
                    string pabteilung = Console.ReadLine();

                    var newPerson = new Person { Id = pid, Name = pname, Alter = palter, Abteilung = pabteilung };
                    ((List<Person>)MenuContext.Lists[listName]).Add(newPerson);
                    FileLogger.LogChange("HINZUFÜGEN", listName, newPerson);
                    UIFormatter.ShowStatus("Person hinzugefügt!");
                    break;

                case "Autos":
                    Console.Write("ID: ");
                    int aid = int.TryParse(Console.ReadLine(), out aid) ? aid : 0;
                    Console.Write("Modell: ");
                    string amodell = Console.ReadLine();
                    Console.Write("AutoBenutzer: ");
                    string aautoBenutzer = Console.ReadLine();
                    Console.Write("Ausgeliehen von: ");
                    string aleihenVon = Console.ReadLine();
                    Console.Write("Ausgeliehen bis: ");
                    string aleihenBis = Console.ReadLine();

                    var newAuto = new Auto { Id = aid, Modell = amodell, AutoBenutzer = aautoBenutzer, LeihenVon = aleihenVon, LeihenBis = aleihenBis };
                    ((List<Auto>)MenuContext.Lists[listName]).Add(newAuto);
                    FileLogger.LogChange("HINZUFÜGEN", listName, newAuto);
                    UIFormatter.ShowStatus("Auto hinzugefügt!");
                    break;

                case "Handys":
                    Console.Write("ID: ");
                    int hid = int.TryParse(Console.ReadLine(), out hid) ? hid : 0;
                    Console.Write("Modell: ");
                    string hmodell = Console.ReadLine();
                    Console.Write("Handybenutzer: ");
                    string hhandyBenutzer = Console.ReadLine();
                    Console.Write("Ausgeliehen von: ");
                    string hleihenVon = Console.ReadLine();
                    Console.Write("Ausgeliehen bis: ");
                    string hleihenBis = Console.ReadLine();

                    var newHandy = new Handy { Id = hid, Modell = hmodell, HandyBenutzer = hhandyBenutzer, LeihenVon = hleihenVon, LeihenBis = hleihenBis};
                    ((List<Handy>)MenuContext.Lists[listName]).Add(newHandy);
                    FileLogger.LogChange("HINZUFÜGEN", listName, newHandy);
                    UIFormatter.ShowStatus("Handy hinzugefügt!");
                    break;

                case "Computer":
                    Console.Write("ID: ");
                    int cid = int.TryParse(Console.ReadLine(), out cid) ? cid : 0;
                    Console.Write("Modell: ");
                    string cmodell = Console.ReadLine();
                    Console.Write("Computerbenutzer: ");
                    string ccomputerBenutzer = Console.ReadLine();
                    Console.Write("Ausgeliehen von: ");
                    string cleihenVon = Console.ReadLine();
                    Console.Write("Ausgeliehen bis: ");
                    string cleihenBis = Console.ReadLine();

                    var newComputer = new Computer { Id = cid, Modell = cmodell, ComputerBenutzer = ccomputerBenutzer, LeihenVon = cleihenVon, LeihenBis = cleihenBis };
                    ((List<Computer>)MenuContext.Lists[listName]).Add(newComputer);
                    FileLogger.LogChange("HINZUFÜGEN", listName, newComputer);
                    UIFormatter.ShowStatus("Computer hinzugefügt!");
                    break;
            }
        }

        private static void EditItem(string listName)
        {
            var list = (IList)MenuContext.Lists[listName];

            if (list.Count == 0)
            {
                UIFormatter.ShowStatus("Keine Einträge vorhanden.");
                return;
            }

            ShowItems(listName);
            Console.Write("Nummer des Eintrags zum Bearbeiten: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= list.Count)
            {
                switch (listName)
                {
                    case "Mitarbeiter":
                        var person = ((List<Person>)list)[index - 1];
                        var beforePerson = new Person { Id = person.Id, Name = person.Name, Alter = person.Alter, Abteilung = person.Abteilung };
                        Console.Write($"Neuer Name ({person.Name}): ");
                        person.Name = Console.ReadLine();
                        Console.Write($"Neues Alter ({person.Alter}): ");
                        person.Alter = int.TryParse(Console.ReadLine(), out int a) ? a : person.Alter;
                        Console.Write($"Neue Abteilung ({person.Abteilung}): ");
                        person.Abteilung = Console.ReadLine();
                        FileLogger.LogChange("BEARBEITEN", listName, new { before = beforePerson, after = person });
                        UIFormatter.ShowStatus("Person bearbeitet.");
                        break;

                    case "Autos":
                        var auto = ((List<Auto>)list)[index - 1];
                        var beforeAuto = new Auto { Id = auto.Id, Modell = auto.Modell, AutoBenutzer = auto.AutoBenutzer, LeihenVon = auto.LeihenVon, LeihenBis = auto.LeihenBis };
                        Console.Write($"Marke ({auto.Modell}): ");
                        auto.Modell = Console.ReadLine();
                        Console.Write($"Neue Benutzer ({auto.AutoBenutzer}): ");
                        auto.AutoBenutzer = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {auto.LeihenVon}");
                        auto.LeihenVon = Console.ReadLine();
                        Console.Write($"Ausgeliehen bis: {auto.LeihenBis}");
                        auto.LeihenBis = Console.ReadLine();
                        FileLogger.LogChange("BEARBEITEN", listName, new { before = beforeAuto, after = auto });
                        UIFormatter.ShowStatus("Auto bearbeitet.");
                        break;

                    case "Handys":
                        var handy = ((List<Handy>)list)[index - 1];
                        var beforeHandy = new Handy { Id = handy.Id, Modell = handy.Modell, HandyBenutzer = handy.HandyBenutzer, LeihenVon = handy.LeihenVon, LeihenBis = handy.LeihenBis };
                        Console.Write($"Marke ({handy.Modell}): ");
                        handy.Modell = Console.ReadLine();
                        Console.Write($"Neue Benutzer ({handy.HandyBenutzer}): ");
                        handy.HandyBenutzer = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {handy.LeihenVon}");
                        handy.LeihenVon = Console.ReadLine();
                        Console.Write($"Ausgeliehen bis: {handy.LeihenBis}");
                        handy.LeihenBis = Console.ReadLine();
                        FileLogger.LogChange("BEARBEITEN", listName, new { before = beforeHandy, after = handy });
                        UIFormatter.ShowStatus("Handy bearbeitet.");
                        break;

                    case "Computer":
                        var computer = ((List<Computer>)list)[index - 1];
                        var beforeComputer = new Computer { Id = computer.Id, Modell = computer.Modell, ComputerBenutzer = computer.ComputerBenutzer, LeihenVon = computer.LeihenVon, LeihenBis = computer.LeihenBis };
                        Console.Write($"Marke ({computer.Modell}): ");
                        computer.Modell = Console.ReadLine();
                        Console.Write($"Neue Benutzer ({computer.ComputerBenutzer}): ");
                        computer.ComputerBenutzer = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {computer.LeihenVon}");
                        computer.LeihenVon = Console.ReadLine();
                        Console.Write($"Ausgeliehen bis: {computer.LeihenBis}");
                        computer.LeihenBis = Console.ReadLine();
                        FileLogger.LogChange("BEARBEITEN", listName, new { before = beforeComputer, after = computer });
                        UIFormatter.ShowStatus("Computer bearbeitet.");
                        break;
                }
            }
        }

        private static void DeleteItem(string listName)
        {
            var list = (IList)MenuContext.Lists[listName];

            if (list.Count == 0)
            {
                UIFormatter.ShowStatus("Keine Einträge vorhanden.");
                return;
            }

            ShowItems(listName);
            Console.Write("Nummer des Eintrags zum Löschen: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= list.Count)
            {
                var removed = list[index - 1];
                list.RemoveAt(index - 1);
                FileLogger.LogChange("LÖSCHEN", listName, removed);
                UIFormatter.ShowStatus("Eintrag gelöscht!");
            }
            else
            {
                UIFormatter.Error("Ungültige Nummer!");
            }
        }
    }
}