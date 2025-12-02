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
            Console.WriteLine("---------- Willkommen zur Verwaltungssoftware! ----------");
            AuthService auth = new AuthService();
            Benutzer user = auth.Login();

            if (user != null)
            {
                Console.WriteLine($"\nErfolgreich eingeloggt als {user.Benutzername} ({user.Rolle}).");
            }
            else
            {
                Console.WriteLine("Login fehlgeschlagen. Bitte überprüfen Sie Ihre Anmeldedaten.");
            }
        }
    }
}
