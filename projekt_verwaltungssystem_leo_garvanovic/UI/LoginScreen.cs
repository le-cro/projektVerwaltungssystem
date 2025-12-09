using projekt_verwaltungssystem_leo_garvanovic.Models;
using projekt_verwaltungssystem_leo_garvanovic.Services;
using System;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    // Login-Screen: sichere Passwort-Eingabe (maskiert) und Rückgabe des authentifizierten Benutzers.
    internal static class LoginScreen
    {
        // Zeigt den Login-Screen; gibt den authentifizierten Benutzer zurück oder null bei Abbruch.
        internal static Benutzer? Show()
        {
            var auth = new AuthService();

            while (true)
            {
                UIFormatter.ClearAndHeader("Anmeldung");

                Console.WriteLine("Bitte melden Sie sich an (oder geben Sie 'q' bei Benutzername ein, um zu beenden)\n");

                Console.Write("Benutzername: ");
                string userInput = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.Equals(userInput, "q", StringComparison.OrdinalIgnoreCase))
                    return null;

                Console.Write("Passwort:     ");
                string password = ReadPassword();

                var user = auth.Authenticate(userInput, password);
                if (user != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nErfolgreich eingeloggt als {user.Benutzername} ({user.Rolle}).");
                    Console.ResetColor();
                    UIFormatter.PressAnyKey();
                    return user;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nLogin fehlgeschlagen. Bitte überprüfen Sie Ihre Anmeldedaten.");
                Console.ResetColor();

                UIFormatter.PressAnyKey();
            }
        }

        // Liest das Passwort maskiert ein (unterstützt Backspace)
        private static string ReadPassword()
        {
            var pwd = new System.Text.StringBuilder();
            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Length--;
                        // entfernt leztes '*' von der Konsole
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pwd.Append(key.KeyChar);
                    Console.Write('*');
                }
            }
            return pwd.ToString();
        }
    }
}