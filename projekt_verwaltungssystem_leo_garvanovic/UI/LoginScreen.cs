using projekt_verwaltungssystem_leo_garvanovic.Models;
using projekt_verwaltungssystem_leo_garvanovic.Services;
using System;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    internal static class LoginScreen
    {
        // Shows the login screen, returns authenticated Benutzer or null if cancelled.
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

        // Read password with masked input, supports Backspace.
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
                        // remove last '*' from console
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