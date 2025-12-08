using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace projekt_verwaltungssystem_leo_garvanovic.Logging
{
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

        private static void Write(string level, string message)
        {
            var line = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ} [{level}] {message}";
            lock (_sync)
            {
                File.AppendAllText(CurrentLogFilePath(), line + Environment.NewLine, Encoding.UTF8);
            }
        }

        public static void Info(string message) => Write("INFO", message);

        public static void Warn(string message) => Write("WARN", message);

        public static void Error(string message, Exception? ex = null)
        {
            if (ex is null)
                Write("ERROR", message);
            else
                Write("ERROR", $"{message} - {ex.GetType().FullName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        public static void LogChange(string action, string listName, object? item = null)
        {
            try
            {
                string payload = item is null ? string.Empty : JsonSerializer.Serialize(item, _jsonOptions);
                Write("CHANGE", $"{action} list={listName} payload={payload}");
            }
            catch (Exception ex)
            {
                // best-effort: if serialization fails, still write a warning
                Write("CHANGE", $"{action} list={listName} (payload serialization failed: {ex.Message})");
            }
        }

        // New: allow manual log entries of arbitrary level (used by admin UI).
        internal static void Manual(string level, string message)
        {
            if (string.IsNullOrWhiteSpace(level))
                level = "INFO";
            level = level.Trim().ToUpperInvariant();
            // Normalize common names
            if (level == "INFORMATION") level = "INFO";
            if (level == "WARNING") level = "WARN";
            Write(level, message);
        }
    }
}