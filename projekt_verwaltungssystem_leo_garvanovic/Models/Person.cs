using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_verwaltungssystem_leo_garvanovic.Models
{
    // Einfaches Modell für Mitarbeiter / Personen in der Liste
    internal class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Alter { get; set; }
        public string Abteilung { get; set; }

        public override string ToString() => $"ID: {Id}, Name: {Name}, Alter: {Alter}, Abteilung: {Abteilung}";
    }
}
