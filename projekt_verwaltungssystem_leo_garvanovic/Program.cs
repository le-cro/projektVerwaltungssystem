using projekt_verwaltungssystem_leo_garvanovic.Models;
using projekt_verwaltungssystem_leo_garvanovic.Services;
using projekt_verwaltungssystem_leo_garvanovic.UI;
using System.Net.Security;
using System.Reflection.Metadata;

namespace projekt_verwaltungssystem_leo_garvanovic
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("------------ Willkommen zur Verwaltungssoftware! ------------");
            AuthService auth = new AuthService();
            Benutzer user = null;

            while (user == null)
            {
                user = auth.Login();

                if (user != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nErfolgreich eingeloggt als {user.Benutzername} ({user.Rolle}).");
                    Console.ForegroundColor = ConsoleColor.White;
                    
                    Menu.ShowMainMenu(user);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nLogin fehlgeschlagen. Bitte überprüfen Sie Ihre Anmeldedaten.\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("-------------------------------------------------------------");
                }
            }
        }
    }
}
