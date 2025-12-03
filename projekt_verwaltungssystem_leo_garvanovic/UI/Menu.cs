using projekt_verwaltungssystem_leo_garvanovic.Models;
using System;
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
                Console.WriteLine("---------- Hauptmenü ----------");

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
                Console.WriteLine("0. Zurück");

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
            Console.WriteLine($"---------- {listName}-Verwaltung ----------");
            Console.WriteLine("1. Anzeigen");

            if (user.Rolle == "Admin")
            {
                Console.WriteLine("2. Hinzufügen");
                Console.WriteLine("3. Bearbeiten");
                Console.WriteLine("4. Löschen");
            }

            Console.WriteLine("0. Zurück");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine($"{listName} anzeigen...");
                    break;
                case "2":
                    if (user.Rolle == "Admin") Console.WriteLine($"{listName} hinzufügen...");
                    else Console.WriteLine("Keine Berechtigung!");
                    break;
                case "3":
                    if (user.Rolle == "Admin") Console.WriteLine($"{listName} bearbeiten...");
                    else Console.WriteLine("Keine Berechtigung!");
                    break;
                case "4":
                    if (user.Rolle == "Admin") Console.WriteLine($"{listName} löschen...");
                    else Console.WriteLine("Keine Berechtigung!");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Ungültige Eingabe!");
                    break;
            }

        }



        private static void BenutzerVerwaltung() => Console.WriteLine("Benutzerverwaltung...");
        private static void ListenVerwaltung() => Console.WriteLine("Listenverwaltung...");
        private static void ExportFunktionen() => Console.WriteLine("Exportfunktionen...");
        private static void SystemLogs() => Console.WriteLine("System-Logs anzeigen...");

    }
}
