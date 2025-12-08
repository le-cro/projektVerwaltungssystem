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
            UIFormatter.ClearAndHeader("Willkommen zur Verwaltungssoftware!");

            // Use the improved login UI
            Benutzer? user = LoginScreen.Show();

            if (user == null)
            {
                // user cancelled
                return;
            }

            Menu.ShowMainMenu(user);
        }
    }
}
