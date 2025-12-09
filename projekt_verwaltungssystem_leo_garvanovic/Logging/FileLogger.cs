using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace projekt_verwaltungssystem_leo_garvanovic.Logging
{
    // Einfache Datei-Logger-Klasse, die Logzeilen in ein tägliches Logfile schreibt.
    // Enthält auch eine Change-Log-Funktion, die Objekte als JSON serialisiert.
    internal static class FileLogger
    {
        private static readonly object _sync = new();
        private static readonly string _logDirectory = @"C:\Temp\Logs";
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };

        private static string CurrentLogFilePath()
        {
            Directory.CreateDirectory(_logDirectory);
            return Path.Combine(_logDirectory, $"app-{DateTime.UtcNow:yyyyMMdd}.log");
        }

        // Internes Schreiben einer Logzeile (Zeitstempel + Level)
        private static void Write(string level, string message)
        {
            var line = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ} [{level}] {message}";
            lock (_sync)
            {
                File.AppendAllText(CurrentLogFilePath(), line + Environment.NewLine, Encoding.UTF8);
            }
        }

        // Hilfs-Methoden für übliche Level
        public static void Info(string message) => Write("INFO", message);

        public static void Warn(string message) => Write("WARN", message);

        public static void Error(string message, Exception? ex = null)
        {
            if (ex is null)
                Write("ERROR", message);
            else
                Write("ERROR", $"{message} - {ex.GetType().FullName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        // Schreibe eine strukturierte Änderungsnachricht (aktion, liste, payload)
        public static void LogChange(string action, string listName, object? item = null)
        {
            try
            {
                string payload = item is null ? string.Empty : JsonSerializer.Serialize(item, _jsonOptions);
                Write("CHANGE", $"{action} list={listName} payload={payload}");
            }
            catch (Exception ex)
            {
                // Best-effort: falls Serialisierung fehlschlägt, schreibe trotzdem eine Warnung
                Write("CHANGE", $"{action} list={listName} (payload serialization failed: {ex.Message})");
            }
        }

        // Ermöglicht manuelles Schreiben eines Logeintrags mit beliebigem Level (Admin-UI)
        internal static void Manual(string level, string message)
        {
            if (string.IsNullOrWhiteSpace(level))
                level = "INFO";
            level = level.Trim().ToUpperInvariant();
            // Normiere häufige Namen
            if (level == "INFORMATION") level = "INFO";
            if (level == "WARNING") level = "WARN";
            Write(level, message);
        }
    }
}