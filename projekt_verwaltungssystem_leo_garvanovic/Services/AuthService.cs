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
            string benutzername = Console.ReadLine()?.Trim().ToLower();

            Console.Write("Passwort:     ");
            string passwort = Console.ReadLine()?.Trim();

            return benutzerListe.FirstOrDefault(b => b.Benutzername == benutzername && b.Passwort == passwort);
        }

        // Added: return readonly view of configured users for administrative UIs and tests.
        public IReadOnlyList<Benutzer> GetAllUsers() => benutzerListe.AsReadOnly();

        // New: authenticate by username/password without performing console I/O (used by UI layer)
        public Benutzer? Authenticate(string benutzername, string passwort)
        {
            if (string.IsNullOrWhiteSpace(benutzername) || string.IsNullOrWhiteSpace(passwort))
                return null;

            // compare username case-insensitively, password must match exactly
            return benutzerListe.FirstOrDefault(b =>
                string.Equals(b.Benutzername, benutzername.Trim(), StringComparison.OrdinalIgnoreCase)
                && b.Passwort == passwort);
        }
    }
}
