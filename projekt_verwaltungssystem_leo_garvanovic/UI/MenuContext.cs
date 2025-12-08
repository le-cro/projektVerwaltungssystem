using projekt_verwaltungssystem_leo_garvanovic.Export;
using projekt_verwaltungssystem_leo_garvanovic.Services;
using projekt_verwaltungssystem_leo_garvanovic.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    // Shared data and services used by the UI classes.
    internal static class MenuContext
    {
        internal static readonly Dictionary<string, object> Lists = new()
        {
            { "Mitarbeiter", new List<Person>() },
            { "Computer", new List<Computer>() },
            { "Handys", new List<Handy>() },
            { "Autos", new List<Auto>() }
        };

        internal static readonly Dictionary<string, Type> ListTypes = new()
        {
            { "Mitarbeiter", typeof(Person) },
            { "Computer",    typeof(Computer) },
            { "Handys",      typeof(Handy) },
            { "Autos",       typeof(Auto) }
        };

        internal static readonly DataService Data = new DataService(Lists, ListTypes);
        internal static readonly CsvExporter Csv = new CsvExporter(Lists, ListTypes);
        internal static char CsvDelimiter = ';';

        // Default directory used for automatic load when opening Listenverwaltung.
        // Adjust this path if you prefer a different default (e.g. use Path.GetTempPath()).
        internal static string DataDirectory = @"C:\Temp\Export";

        // Prevent repeated automatic loads on subsequent opens of the menu.
        internal static bool DataLoaded = false;

        internal static IList GetIList(string listName) => (IList)Lists[listName];
        internal static Type GetItemType(string listName) => ListTypes[listName];
    }
}