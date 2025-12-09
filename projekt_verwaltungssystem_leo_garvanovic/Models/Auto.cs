using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_verwaltungssystem_leo_garvanovic.Models
{
    // Modell für Auto-Einträge
    internal class Auto
    {
        public int Id { get; set; }
        public string Modell { get; set; }
        public string AutoBenutzer { get; set; }
        public string LeihenVon { get; set; }
        public string LeihenBis { get; set; }

        public override string ToString() => $"ID: {Id}, Modell: {Modell}, AutoBenutzer: {AutoBenutzer}, Ausgeliehen von: {LeihenVon} bis {LeihenBis}";
    }
}

