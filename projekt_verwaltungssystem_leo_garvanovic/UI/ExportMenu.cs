using System;
using System.Linq;
using projekt_verwaltungssystem_leo_garvanovic.Logging;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    public static class ExportMenu
    {
        public static void ExportFunktionen()
        {
            while (true)
            {
                UIFormatter.ClearAndHeader("Export/Import");
                UIFormatter.PrintOptions(
                    $"(Trennzeichen: '{MenuContext.CsvDelimiter}')",
                    "1. Liste exportieren",
                    "2. Alle Listen exportieren",
                    "3. Liste importieren",
                    "4. Alle Listen importieren",
                    "5. Trennzeichen wechseln (, / ;)",
                    "0. Zurück"
                );

                switch (UIFormatter.ReadOption())
                {
                    case "1":
                        {
                            var list = PromptListName();
                            if (list == null) break;
                            Console.Write($"Pfad (z.B. C:\\Temp\\{list}.csv): ");
                            var path = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(path)) break;
                            try
                            {
                                MenuContext.Csv.SaveList(list, path.Trim(), MenuContext.CsvDelimiter, includeHeader: true);
                                FileLogger.Info($"Export der Liste '{list}' -> {path}");
                                UIFormatter.ShowStatus($"Export '{list}' -> {path}");
                                UIFormatter.PressAnyKey();
                            }
                            catch (Exception ex)
                            {
                                FileLogger.Error($"Export fehlgeschlagen für '{list}' -> {path}", ex);
                                UIFormatter.Error($"Export fehlgeschlagen: {ex.Message}");
                                UIFormatter.PressAnyKey();
                            }
                        }
                        break;

                    case "2":
                        {
                            Console.Write("Ordner (z.B. C:\\Temp\\Export): ");
                            var dir = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(dir)) break;
                            try
                            {
                                MenuContext.Csv.SaveAll(dir.Trim(), MenuContext.CsvDelimiter);
                                FileLogger.Info($"Export aller Listen -> {dir}");
                                UIFormatter.ShowStatus($"Alle Listen exportiert -> {dir}");
                                UIFormatter.PressAnyKey();
                            }
                            catch (Exception ex)
                            {
                                FileLogger.Error($"Export aller Listen fehlgeschlagen -> {dir}", ex);
                                UIFormatter.Error($"Export fehlgeschlagen: {ex.Message}");
                                UIFormatter.PressAnyKey();
                            }
                        }
                        break;

                    case "3":
                        {
                            var list = PromptListName();
                            if (list == null) break;
                            Console.Write($"Pfad (z.B. C:\\Temp\\{list}.csv): ");
                            var path = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(path)) break;
                            try
                            {
                                bool clear = PromptYesNo("Vorher löschen? (j/n): ");
                                int n = MenuContext.Csv.LoadList(list, path.Trim(), MenuContext.CsvDelimiter, clear);
                                FileLogger.Info($"Importiert {n} Zeilen in '{list}' von {path}");
                                UIFormatter.ShowStatus($"{n} Zeilen importiert in '{list}'");
                                UIFormatter.PressAnyKey();
                            }
                            catch (Exception ex)
                            {
                                FileLogger.Error($"Import fehlgeschlagen für '{list}' <- {path}", ex);
                                UIFormatter.Error($"Import fehlgeschlagen: {ex.Message}");
                                UIFormatter.PressAnyKey();
                            }
                        }
                        break;

                    case "4":
                        {
                            Console.Write("Ordner (z.B. C:\\Temp\\Export): ");
                            var dir = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(dir)) break;
                            try
                            {
                                bool clear = PromptYesNo("Alle vorher löschen? (j/n): ");
                                int n = MenuContext.Csv.LoadAll(dir.Trim(), MenuContext.CsvDelimiter, clear);
                                FileLogger.Info($"Importiert {n} Zeilen aus {dir} in alle Listen");
                                UIFormatter.ShowStatus($"{n} Zeilen aus allen CSVs importiert");
                                UIFormatter.PressAnyKey();
                            }
                            catch (Exception ex)
                            {
                                FileLogger.Error($"Import aller Listen fehlgeschlagen <- {dir}", ex);
                                UIFormatter.Error($"Import fehlgeschlagen: {ex.Message}");
                                UIFormatter.PressAnyKey();
                            }
                        }
                        break;

                    case "5":
                        MenuContext.CsvDelimiter = (MenuContext.CsvDelimiter == ';') ? ',' : ';';
                        FileLogger.Info($"CSV-Trennzeichen geändert auf '{MenuContext.CsvDelimiter}'");
                        UIFormatter.ShowStatus($"Trennzeichen geändert auf '{MenuContext.CsvDelimiter}'");
                        UIFormatter.PressAnyKey();
                        break;

                    case "0":
                        return;

                    default:
                        UIFormatter.Error("Ungültige Eingabe.");
                        UIFormatter.PressAnyKey();
                        break;
                }
            }
        }

        private static string? PromptListName()
        {
            var names = MenuContext.Lists.Keys.ToList();
            if (names.Count == 0) { UIFormatter.ShowStatus("Keine Listen vorhanden."); UIFormatter.PressAnyKey(); return null; }

            UIFormatter.ShowStatus("Listen:");
            for (int i = 0; i < names.Count; i++) Console.WriteLine($"  {i + 1}. {names[i]}");
            Console.Write("Auswahl #: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > names.Count) { UIFormatter.Error("Ungültig."); UIFormatter.PressAnyKey(); return null; }
            return names[idx - 1];
        }

        private static bool PromptYesNo(string prompt)
        {
            Console.Write(prompt);
            var s = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            return s is "j" or "ja" or "y" or "yes";
        }
    }
}