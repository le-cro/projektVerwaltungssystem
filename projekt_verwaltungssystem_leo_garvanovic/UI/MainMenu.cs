using projekt_verwaltungssystem_leo_garvanovic.Models;
using projekt_verwaltungssystem_leo_garvanovic.Services;
using projekt_verwaltungssystem_leo_garvanovic.Logging;
using System;
using System.IO;
using System.Linq;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    // Hauptmenü: zeigt administrative Optionen für Admins und eingeschränkte Optionen für normale User.
    public static class MainMenu
    {
        public static void Show(Benutzer user)
        {
            bool running = true;

            while (running)
            {
                UIFormatter.ClearAndHeader("Hauptmenü");

                // Admins sehen zusätzliche Menüpunkte (Benutzerverwaltung, System-Logs)
                if (user.Rolle == "Admin")
                {
                    UIFormatter.PrintOptions(
                        "1. Benutzerverwaltung",
                        "2. Listenverwaltung",
                        "3. Export",
                        "4. System-Logs",
                        "0. Logout"
                    );
                }
                else
                {
                    UIFormatter.PrintOptions(
                        "1. Listenverwaltung",
                        "2. Export",
                        "0. Logout"
                    );
                }

                string choice = UIFormatter.ReadOption();

                if (user.Rolle == "Admin")
                {
                    switch (choice)
                    {
                        case "1":
                            BenutzerVerwaltung();
                            break;

                        case "2":
                            ListMenu.ShowListenMenu(user);
                            break;

                        case "3":
                            ExportMenu.ExportFunktionen();
                            break;

                        case "4":
                            SystemLogs();
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
                else
                {
                    switch (choice)
                    {
                        case "1":
                            ListMenu.ShowListenMenu(user);
                            break;

                        case "2":
                            ExportMenu.ExportFunktionen();
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
        }

        // Zeigt die aktuelle, statisch konfigurierte Benutzerliste an (read-only)
        private static void BenutzerVerwaltung()
        {
            var auth = new AuthService();
            var users = auth.GetAllUsers();

            UIFormatter.ClearAndHeader("Benutzerverwaltung");

            if (users == null || users.Count == 0)
            {
                UIFormatter.ShowStatus("Keine Benutzer vorhanden.");
            }
            else
            {
                UIFormatter.ShowStatus("Alle Benutzer:");
                int i = 1;
                foreach (var b in users)
                {
                    Console.WriteLine($"  {i++}. {b.Benutzername}  (Rolle: {b.Rolle})");
                }
            }

            Console.WriteLine();
            ConsoleUtils.Pause();
        }

        // Admin-Funktion: Logdateien anzeigen, Ordner öffnen oder neuen Logeintrag erzeugen.
        private static void SystemLogs()
        {
            const string logDir = @"C:\Temp\Logs";
            const int tailLines = 200;

            while (true)
            {
                UIFormatter.ClearAndHeader("System-Logs");
                UIFormatter.PrintOptions(
                    $"1. Neueste Logdatei anzeigen (letzte {tailLines} Zeilen)",
                    "2. Log-Ordner öffnen",
                    "3. Neuen Logeintrag erstellen",
                    "0. Zurück"
                );

                var choice = UIFormatter.ReadOption();
                if (choice == "0") return;

                if (choice == "2")
                {
                    try
                    {
                        if (!Directory.Exists(logDir))
                        {
                            UIFormatter.Error("Log-Ordner existiert nicht.");
                            UIFormatter.PressAnyKey();
                            continue;
                        }

                        var psi = new System.Diagnostics.ProcessStartInfo("explorer.exe", logDir) { UseShellExecute = true };
                        System.Diagnostics.Process.Start(psi);
                    }
                    catch (Exception ex)
                    {
                        UIFormatter.Error("Fehler beim Öffnen des Log-Ordners: " + ex.Message);
                    }
                    UIFormatter.PressAnyKey();
                    continue;
                }

                if (choice == "1")
                {
                    try
                    {
                        if (!Directory.Exists(logDir))
                        {
                            UIFormatter.Error("Log-Ordner existiert nicht.");
                            UIFormatter.PressAnyKey();
                            continue;
                        }

                        var files = Directory.GetFiles(logDir, "*.log");
                        if (files.Length == 0)
                        {
                            UIFormatter.ShowStatus("Keine Logdateien gefunden.");
                            UIFormatter.PressAnyKey();
                            continue;
                        }

                        var latest = files.OrderByDescending(f => File.GetLastWriteTimeUtc(f)).First();
                        var lines = File.ReadAllLines(latest);
                        UIFormatter.ShowStatus($"--- {Path.GetFileName(latest)} (zeige letzte {tailLines} Zeilen) ---\n");

                        var start = Math.Max(0, lines.Length - tailLines);
                        for (int i = start; i < lines.Length; i++)
                            Console.WriteLine(lines[i]);
                    }
                    catch (Exception ex)
                    {
                        UIFormatter.Error("Fehler beim Lesen der Logdatei: " + ex.Message);
                    }
                    UIFormatter.PressAnyKey();
                    continue;
                }

                if (choice == "3")
                {
                    UIFormatter.ShowStatus("Neuen Logeintrag erstellen");
                    UIFormatter.ShowStatus("Level wählen: 1=INFO, 2=WARN, 3=ERROR");
                    var lvlChoice = UIFormatter.ReadOption("Auswahl: ");
                    string level = lvlChoice switch { "2" => "WARN", "3" => "ERROR", _ => "INFO" };

                    Console.Write("Nachricht eingeben: ");
                    var message = Console.ReadLine() ?? string.Empty;

                    try
                    {
                        FileLogger.Manual(level, message);
                        UIFormatter.ShowStatus("Logeintrag wurde erstellt.");
                    }
                    catch (Exception ex)
                    {
                        UIFormatter.Error("Fehler beim Schreiben des Logeintrags: " + ex.Message);
                        FileLogger.Error("Fehler beim manuellen Logeintrag", ex);
                    }

                    UIFormatter.PressAnyKey();
                    continue;
                }

                UIFormatter.Error("Ungültige Auswahl.");
                UIFormatter.PressAnyKey();
            }
        }
    }
}