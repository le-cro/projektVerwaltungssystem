using System;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    internal static class UIFormatter
    {
        // Clear screen and draw a boxed header with centered title (uses Unicode box characters).
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

            // Extra spacing to avoid option and input being shown on same line
            Console.WriteLine();
        }

        // Print options in a consistent color and with a small indent
        internal static void PrintOptions(params string[] options)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var o in options)
            {
                Console.WriteLine("  " + o);
            }
            Console.ResetColor();
            Console.WriteLine(); // separate options from prompt
        }

        // Read an option; leaves an empty line after reading to separate prompt and subsequent content
        internal static string ReadOption(string prompt = "Auswahl: ")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(prompt);
            Console.ResetColor();

            var s = Console.ReadLine() ?? string.Empty;

            // ensure visual separation after choice
            Console.WriteLine();
            return s.Trim();
        }

        internal static void ShowStatus(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(text);
            Console.ResetColor();
        }

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