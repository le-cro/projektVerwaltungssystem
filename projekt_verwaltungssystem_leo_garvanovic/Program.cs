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
            // Kurze Willkommensanzeige und Header
            UIFormatter.ClearAndHeader("Willkommen zur Verwaltungssoftware!");

            // Verbessertes Login-UI verwenden.
            // LoginScreen.Show() zeigt das Eingabeformular an und gibt bei Erfolg ein Benutzerobjekt zurück.
            Benutzer? user = LoginScreen.Show();

            if (user == null)
            {
                // Benutzer hat mit 'q' abgebrochen -> Programm beenden
                return;
            }

            // Hauptmenü anzeigen und an den angemeldeten Benutzer binden
            Menu.ShowMainMenu(user);
        }
    }
}
