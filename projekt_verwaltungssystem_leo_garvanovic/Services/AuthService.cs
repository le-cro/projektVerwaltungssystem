using projekt_verwaltungssystem_leo_garvanovic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_verwaltungssystem_leo_garvanovic.Services
{
    // Einfache Authentifizierungs-Logik mit einer statisch konfigurierten Benutzerliste.
    // In einer realen Anwendung würde hier eine sichere Speicherung / Hashing verwendet werden.
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

        // Legacy-Methode, die Konsoleneingaben liest (nicht von UI verwendet).
        public Benutzer Login()
        {
            Console.Write("Benutzername: ");
            string benutzername = Console.ReadLine()?.Trim().ToLower();

            Console.Write("Passwort:     ");
            string passwort = Console.ReadLine()?.Trim();

            return benutzerListe.FirstOrDefault(b => b.Benutzername == benutzername && b.Passwort == passwort);
        }

        // Liefert eine readonly-Ansicht der konfigurierten Benutzer (für Admin-UI und Tests).
        public IReadOnlyList<Benutzer> GetAllUsers() => benutzerListe.AsReadOnly();

        // Authentifiziert ohne Konsolen-I/O (verwendet durch LoginScreen)
        public Benutzer? Authenticate(string benutzername, string passwort)
        {
            if (string.IsNullOrWhiteSpace(benutzername) || string.IsNullOrWhiteSpace(passwort))
                return null;

            // Benutzername case-insensitiv vergleichen, Passwort exakt.
            return benutzerListe.FirstOrDefault(b =>
                string.Equals(b.Benutzername, benutzername.Trim(), StringComparison.OrdinalIgnoreCase)
                && b.Passwort == passwort);
        }
    }
}
