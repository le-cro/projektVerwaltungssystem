using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_verwaltungssystem_leo_garvanovic.Models
{
    internal class Handy
    {
        public int Id { get; set; }
        public string Modell { get; set; }
        public string HandyBenutzer { get; set; }
        public int LeihenVon { get; set; }
        public int LeihenBis { get; set; }

        public override string ToString() => $"ID: {Id}, Modell: {Modell}, HandyBenutzer: {HandyBenutzer}, Ausgeliehen von: {LeihenVon} bis {LeihenBis}";
    }
}
