using projekt_verwaltungssystem_leo_garvanovic.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    public static class Menu
    {
        public static void ShowMainMenu(Benutzer user)
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("---------- Hauptmenü ----------\n");

                if (user.Rolle == "Admin")
                {
                    Console.WriteLine("1. Benutzerverwaltung");
                    Console.WriteLine("2. Listenverwaltung");
                    Console.WriteLine("3. Export");
                    Console.WriteLine("4. System-Logs");
                    Console.WriteLine("0. Logout\n");
                }
                else
                {
                    Console.WriteLine("1. Listenverwaltung");
                    Console.WriteLine("2. Export");
                    Console.WriteLine("0. Logout\n");
                }
                

                string choice = Console.ReadLine();

                if (user.Rolle == "Admin")
                {
                    switch (choice)
                    {
                        case "1":
                            BenutzerVerwaltung();
                            break;

                        case "2":
                            Menu.ShowListenMenu(user);
                            break;

                        case "3":
                            ExportFunktionen();
                            break;

                        case "4":
                            SystemLogs();
                            break;

                        case "0":
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Ungültige eingabe! Drücken Sie eine Taste, um fortzufahren...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    switch (choice)
                    {
                        case "1":
                            ListenVerwaltung();
                            break;

                        case "2":
                            ExportFunktionen();
                            break;

                        case "0":
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Ungültige eingabe! Drücken Sie eine Taste, um fortzufahren...");
                            Console.ReadKey();
                            break;
                    }
                }
            }
        }

        //private static Dictionary<string, object> listen = new Dictionary<string, object>()
        private static readonly Dictionary<string, System.Collections.IList> listen = new()
        {
            { "Mitarbeiter", new List<Person>() },
            { "Computer", new List<Computer>() },
            { "Handys", new List<Handy>() },
            { "Autos", new List<Auto>() }
        };

        private static readonly Dictionary<string, Type> listTypes = new()
        {
            { "Mitarbeiter", typeof(Person) },
            { "Computer",    typeof(Computer) },
            { "Handys",      typeof(Handy) },
            { "Autos",       typeof(Auto) }
        };

        private static IList GetIList(string listName) => (IList)listen[listName];
        private static Type GetItemType(string listName) => listTypes[listName];

        private static void ShowListenMenu(Benutzer user)
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("---------- Listenverwaltung ----------");
                Console.WriteLine("1. Mitarbeiter");
                Console.WriteLine("2. Computer");
                Console.WriteLine("3. Handys");
                Console.WriteLine("4. Autos");
                Console.WriteLine("0. Zurück\n");

                string choice = Console.ReadLine();

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
                        Console.WriteLine("Ungültige eingabe! Drücken Sie eine Taste, um fortzufahren...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void ManageList(string listName, Benutzer user)
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();
                Console.WriteLine($"---------- {listName}-Verwaltung ----------");
                Console.WriteLine("1. Anzeigen");

                if (user.Rolle == "Admin")
                {
                    Console.WriteLine("2. Hinzufügen");
                    Console.WriteLine("3. Bearbeiten");
                    Console.WriteLine("4. Löschen");
                }

                Console.WriteLine("0. Zurück\n");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowBrowseMenu(listName); // Untermenü von Anzeigen
                        break;
                    case "2":
                        if (user.Rolle == "Admin") AddItem(listName);
                        else Console.WriteLine("Keine Berechtigung!");
                        Pause();
                        break;
                    case "3":
                        if (user.Rolle == "Admin") EditItem(listName);
                        else Console.WriteLine("Keine Berechtigung!");
                        Pause();
                        break;
                    case "4":
                        if (user.Rolle == "Admin") DeleteItem(listName);
                        else Console.WriteLine("Keine Berechtigung!");
                        Pause();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Ungültige Eingabe!");
                        Pause();
                        break;
                }

                if (!back)
                {
                    Console.WriteLine("Drücken Sie eine Taste, um fortzufahren...");
                    Console.ReadKey();
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine("\nDrücken Sie eine Taste, um fortzufahren...");
            Console.ReadKey();
        }


        private static void ShowBrowseMenu(string listName)
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine($"---------- {listName}: Anzeigen ----------");
                Console.WriteLine("1. Alle Einträge anzeigen");
                Console.WriteLine("2. Suchen (Freitext über alle Eigenschaften)");
                Console.WriteLine("3. Filtern (Eigenschaft + Operator)");
                Console.WriteLine("0. Zurück");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowItems(listName);
                        Pause();
                        break;

                    case "2":
                        SearchItems(listName);
                        Pause();
                        break;

                    case "3":
                        FilterItems(listName);
                        Pause();
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("Ungültige Eingabe!");
                        Pause();
                        break;
                }
            }
        }

        private static void ShowItems(string listName)
        {
            var list = GetIList(listName);
            Console.WriteLine($"{listName} anzeigen:");

            if (list.Count == 0)
            {
                Console.WriteLine("Keine Einträge vorhanden.");
                return;
            }

            int i = 1;
            foreach (var item in list)
                Console.WriteLine($"{i++}. {item}");
        }

        private static void SearchItems(string listName)
        {
            var list = GetIList(listName);
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
            var list = GetIList(listName);
            var type = GetItemType(listName);
            var props = type.GetProperties().Where(p => p.CanRead).ToArray();

            if (props.Length == 0)
            {
                Console.WriteLine("Keine filterbaren Eigenschaften gefunden.");
                return;
            }

            Console.WriteLine("Verfügbare Eigenschaften:");
            for (int i = 0; i < props.Length; i++)
                Console.WriteLine($"{i + 1}. {props[i].Name} ({props[i].PropertyType.Name})");

            Console.Write("Eigenschaft auswählen (Nummer): ");
            if (!int.TryParse(Console.ReadLine(), out int pIndex) || pIndex < 1 || pIndex > props.Length)
            {
                Console.WriteLine("Ungültige Auswahl.");
                return;
            }

            var prop = props[pIndex - 1];
            IEnumerable<object> results = Enumerable.Empty<object>();

            if (prop.PropertyType == typeof(string))
            { }
        }

        private static void AddItem(string listName)
        {
            switch (listName)
            {
                case "Mitarbeiter":
                    Console.Write("ID: ");
                    int pid = int.Parse(Console.ReadLine());
                    Console.Write("Name: ");
                    string pname = Console.ReadLine();
                    Console.Write("Alter: ");
                    int palter = int.Parse(Console.ReadLine());
                    Console.Write("Abteilung: ");
                    string pabteilung = Console.ReadLine();

                    ((List<Person>)listen[listName]).Add(new Person { Id = pid, Name = pname, Alter = palter, Abteilung = pabteilung });
                    Console.WriteLine("Person hinzugefügt!");
                    break;

                case "Autos":
                    Console.Write("ID: ");
                    int aid = int.Parse(Console.ReadLine());
                    Console.Write("Modell: ");
                    string amodell = Console.ReadLine();
                    Console.Write("AutoBenutzer: ");
                    string aautoBenutzer = Console.ReadLine();
                    Console.WriteLine("Ausgeliehen von: ");
                    string aleihenVon = Console.ReadLine();
                    Console.WriteLine("Ausgeliehen bis: ");
                    string aleihenBis = Console.ReadLine();

                    ((List<Auto>)listen[listName]).Add(new Auto { Id = aid, Modell = amodell, AutoBenutzer = aautoBenutzer, LeihenVon = aleihenVon, LeihenBis = aleihenBis });
                    Console.WriteLine("Auto hinzugefügt!");
                    break;

                case "Handys":
                    Console.Write("ID: ");
                    int hid = int.Parse(Console.ReadLine());
                    Console.Write("Modell: ");
                    string hmodell = Console.ReadLine();
                    Console.Write("Handybenutzer: ");
                    string hhandyBenutzer = Console.ReadLine();
                    Console.WriteLine("Ausgeliehen von: ");
                    string hleihenVon = Console.ReadLine();
                    Console.WriteLine("Ausgeliehen bis: ");
                    string hleihenBis = Console.ReadLine();

                    ((List<Handy>)listen[listName]).Add(new Handy { Id = hid, Modell = hmodell, HandyBenutzer = hhandyBenutzer, LeihenVon = hleihenVon, LeihenBis = hleihenBis});
                    Console.WriteLine("Handy hinzugefügt!");
                    break;

                case "Computer":
                    Console.Write("ID: ");
                    int cid = int.Parse(Console.ReadLine());
                    Console.Write("Modell: ");
                    string cmodell = Console.ReadLine();
                    Console.Write("Computerbenutzer: ");
                    string ccomputerBenutzer = Console.ReadLine();
                    Console.WriteLine("Ausgeliehen von: ");
                    string cleihenVon = Console.ReadLine();
                    Console.WriteLine("Ausgeliehen bis: ");
                    string cleihenBis = Console.ReadLine();

                    ((List<Computer>)listen[listName]).Add(new Computer { Id = cid, Modell = cmodell, ComputerBenutzer = ccomputerBenutzer, LeihenVon = cleihenVon, LeihenBis = cleihenBis });
                    Console.WriteLine("Computer hinzugefügt!");
                    break;
            }
        }



        private static void EditItem(string listName)
        {
            var list = (System.Collections.IList)listen[listName];

            if (list.Count == 0)
            {
                Console.WriteLine("Keine Einträge vorhanden.");
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
                        Console.Write($"Neuer Name ({person.Name}): ");
                        person.Name = Console.ReadLine();
                        Console.Write($"Neues Alter ({person.Alter}): ");
                        person.Alter = int.Parse(Console.ReadLine());
                        Console.Write($"Neue Abteilung ({person.Abteilung}): ");
                        person.Abteilung = Console.ReadLine();
                        break;

                    case "Autos":
                        var auto = ((List<Auto>)list)[index - 1];
                        Console.Write($"Marke ({auto.Modell}): ");
                        auto.Modell = Console.ReadLine();
                        Console.Write($"Neue Benutzer ({auto.AutoBenutzer}): ");
                        auto.AutoBenutzer = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {auto.LeihenVon}");
                        auto.LeihenVon = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {auto.LeihenBis}");
                        auto.LeihenBis = Console.ReadLine();
                        break;

                    case "Handys":
                        var handy = ((List<Handy>)list)[index - 1];
                        Console.Write($"Marke ({handy.Modell}): ");
                        handy.Modell = Console.ReadLine();
                        Console.Write($"Neue Benutzer ({handy.HandyBenutzer}): ");
                        handy.HandyBenutzer = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {handy.LeihenVon}");
                        handy.LeihenVon = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {handy.LeihenBis}");
                        handy.LeihenBis = Console.ReadLine();
                        break;

                    case "Computer":
                        var computer = ((List<Computer>)list)[index - 1];
                        Console.Write($"Marke ({computer.Modell}): ");
                        computer.Modell = Console.ReadLine();
                        Console.Write($"Neue Benutzer ({computer.ComputerBenutzer}): ");
                        computer.ComputerBenutzer = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {computer.LeihenVon}");
                        computer.LeihenVon = Console.ReadLine();
                        Console.Write($"Ausgeliehen von: {computer.LeihenBis}");
                        computer.LeihenBis = Console.ReadLine();
                        break;
                }
            }
        }

        private static void DeleteItem(string listName)
        {
            var list = (System.Collections.IList)listen[listName];

            if (list.Count == 0)
            {
                Console.WriteLine("Keine Einträge vorhanden.");
                return;
            }

            ShowItems(listName);
            Console.Write("Nummer des Eintrags zum Löschen: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= list.Count)
            {
                list.RemoveAt(index - 1);
                Console.WriteLine("Eintrag gelöscht!");
            }
            else
            {
                Console.WriteLine("Ungültige Nummer!");
            }
        }



        private static void BenutzerVerwaltung() => Console.WriteLine("Benutzerverwaltung...");
        private static void ListenVerwaltung() => Console.WriteLine("Listenverwaltung...");
        private static void ExportFunktionen() => Console.WriteLine("Exportfunktionen...");
        private static void SystemLogs() => Console.WriteLine("System-Logs anzeigen...");

    }
}
