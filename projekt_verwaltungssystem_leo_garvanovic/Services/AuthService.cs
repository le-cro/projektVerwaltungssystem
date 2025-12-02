using projekt_verwaltungssystem_leo_garvanovic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_verwaltungssystem_leo_garvanovic.Services
{
    public class AuthService
    {
        private List<Benutzer> benutzerListe = new List<Benutzer>
        {
            new Benutzer("admin", "1234", "Admin" ),
            new Benutzer("user", "password", "User" ),
            new Benutzer("test", "test", "User" ),
            new Benutzer("leo", "pass", "User" ),
            new Benutzer("administrator", "admin", "Admin" )
        };

        public Benutzer Login()
        {
            Console.Write("Benutzername: ");
            string benutzername = Console.ReadLine();
            string benutzernameInput = benutzername.ToLower();

            Console.Write("Passwort:     ");
            string passwort = Console.ReadLine();

            return benutzerListe.FirstOrDefault(b => b.Benutzername == benutzernameInput && b.Passwort == passwort);
        }
    }
}
