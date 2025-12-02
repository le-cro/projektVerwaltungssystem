using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_verwaltungssystem_leo_garvanovic.Models
{
    public class Benutzer
    {
        public string Benutzername { get; set; }
        public string Passwort { get; set; }
        public string Rolle { get; set; }

        public Benutzer(string benutzername, string passwort, string rolle)
        {
            Benutzername = benutzername;
            Passwort = passwort;
            Rolle = rolle;
        }
    }
}
