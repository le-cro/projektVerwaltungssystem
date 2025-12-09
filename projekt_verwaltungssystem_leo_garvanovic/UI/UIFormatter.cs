using System;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    // Kleine Hilfsklasse für konsistente Konsolen-Ausgabe (Header, Optionen, Farben)
    internal static class UIFormatter
    {
        // Bildschirm löschen und eine boxartige Überschrift mit zentriertem Titel zeichnen.
        internal static void ClearAndHeader(string title)
        {
            Console.Clear();

            int width;
            try { width = Math.Min(Math.Max(40, Console.WindowWidth), 100); }
            catch { width = 80; }

            var borderColor = ConsoleColor.DarkBlue;
            var titleColor = ConsoleColor.White;

            Console.ForegroundColor = borderColor;
            Console.WriteLine("┌" + new string('─', width - 2) + "┐");
            Console.ResetColor();

            Console.ForegroundColor = titleColor;
            string content = $" {title} ";
            int inner = Math.Max(0, width - 2 - content.Length);
            int left = inner / 2;
            int right = inner - left;
            Console.WriteLine("│" + new string(' ', left) + content + new string(' ', right) + "│");
            Console.ResetColor();

            Console.ForegroundColor = borderColor;
            Console.WriteLine("└" + new string('─', width - 2) + "┘");
            Console.ResetColor();

            // Zusätzlicher Abstand, damit Eingabeaufforderung nicht gleich an der selben Zeile hängt
            Console.WriteLine();
        }

        // Optionen in einer einheitlichen Farbe ausgeben
        internal static void PrintOptions(params string[] options)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var o in options)
            {
                Console.WriteLine("  " + o);
            }
            Console.ResetColor();
            Console.WriteLine(); // Trennung zur Eingabe
        }

        // Liest eine Option und sorgt für Trennung in der Anzeige
        internal static string ReadOption(string prompt = "Auswahl: ")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(prompt);
            Console.ResetColor();

            var s = Console.ReadLine() ?? string.Empty;

            // visuelle Trennung nach der Eingabe
            Console.WriteLine();
            return s.Trim();
        }

        internal static void ShowStatus(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        // Standard-Hinweis für "Press any key"
        internal static void PressAnyKey(string hint = "Drücken Sie eine Taste, um fortzufahren...")
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(hint);
            Console.ResetColor();
            Console.ReadKey(true);
            Console.WriteLine();
        }

        internal static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}