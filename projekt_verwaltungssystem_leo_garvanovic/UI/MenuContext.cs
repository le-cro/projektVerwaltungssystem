using projekt_verwaltungssystem_leo_garvanovic.Export;
using projekt_verwaltungssystem_leo_garvanovic.Services;
using projekt_verwaltungssystem_leo_garvanovic.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    // Gemeinsamer Kontext, der von allen UI-Komponenten benutzt wird.
    // Enthält die in-memory-Listen, deren Typen, sowie die Data- und CSV-Helper.
    internal static class MenuContext
    {
        // In-Memory-Listen (Key = Anzeigename, Value = List<T> als object)
        internal static readonly Dictionary<string, object> Lists = new()
        {
            { "Mitarbeiter", new List<Person>() },
            { "Computer", new List<Computer>() },
            { "Handys", new List<Handy>() },
            { "Autos", new List<Auto>() }
        };

        // Mapping von Listenname -> Elementtyp (z.B. "Mitarbeiter" => typeof(Person))
        internal static readonly Dictionary<string, Type> ListTypes = new()
        {
            { "Mitarbeiter", typeof(Person) },
            { "Computer",    typeof(Computer) },
            { "Handys",      typeof(Handy) },
            { "Autos",       typeof(Auto) }
        };

        // Instanzen der Service-Klassen, bereitgestellt für die UI
        internal static readonly DataService Data = new DataService(Lists, ListTypes);
        internal static readonly CsvExporter Csv = new CsvExporter(Lists, ListTypes);

        // CSV-Trennzeichen (kann im UI gewechselt werden)
        internal static char CsvDelimiter = ';';

        // Standardverzeichnis zum automatischen Laden/Speichern (kann angepasst werden)
        internal static string DataDirectory = @"C:\Temp\Export";

        // Flag, damit automatische einmalige Datenladung nicht wiederholt wird
        internal static bool DataLoaded = false;

        // Hilfszugriffe für UI (gibt IList oder Type zurück)
        internal static IList GetIList(string listName) => (IList)Lists[listName];
        internal static Type GetItemType(string listName) => ListTypes[listName];
    }
}