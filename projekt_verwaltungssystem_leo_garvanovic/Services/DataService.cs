using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using projekt_verwaltungssystem_leo_garvanovic.Logging;


namespace projekt_verwaltungssystem_leo_garvanovic.Services
{
    // Service, der das Speichern / Laden von Liste<T> in CSV-Dateien kapselt.
    // Arbeitet generisch über Reflection, damit es mit beliebigen Modeltypen funktioniert.
    public sealed class DataService
    {
        
        private readonly Dictionary<string, object> _listen;
        private readonly Dictionary<string, Type> _types;

        public DataService(Dictionary<string, object> listen, Dictionary<string, Type> listTypes)
        {
            _listen = listen ?? throw new ArgumentNullException(nameof(listen));
            _types = listTypes ?? throw new ArgumentNullException(nameof(listTypes));
        }

        // Öffentlich lesbare Views für Debug/Anzeigezwecke
        public IReadOnlyDictionary<string, object> Lists => _listen;
        public IReadOnlyDictionary<string, Type> Types => _types;

        // Hilfsmethoden zum schnellen Zugriff
        public List<T> GetList<T>(string name) => (List<T>)_listen[name];
        public Type GetItemType(string name) => _types[name];


        // ---------- PUBLIC API ----------

        // Speichert eine einzelne Liste in eine CSV-Datei. Schreibt Header (Eigenschaftsnamen) optional.
        public void SaveList(string listName, string filePath, char delimiter = ';', bool includeHeader = true)
        {
            EnsureList(listName);
            FileLogger.Info($"Speichere Liste gestartet: {listName} -> {filePath}");

            try
            {
                var type = _types[listName];
                var listObj = _listen[listName];
                var items = Enumerate(listObj);

                var props = GetReadableProps(type);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var sw = new StreamWriter(filePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

                if (includeHeader)
                {
                    string header = string.Join(delimiter, props.Select(p => EscapeCsv(p.Name, delimiter)));
                    sw.WriteLine(header);
                }

                foreach (var item in items)
                {
                    var cells = props.Select(p =>
                    {
                        object? raw = p.GetValue(item);
                        string s = Convert.ToString(raw, CultureInfo.InvariantCulture) ?? string.Empty;
                        return EscapeCsv(s, delimiter);
                    });
                    sw.WriteLine(string.Join(delimiter, cells));
                }

                FileLogger.Info($"Speichere Liste erfolgreich: {listName} -> {filePath}");
            }
            catch (Exception ex)
            {
                FileLogger.Error($"Speichern der Liste fehlgeschlagen: {listName} -> {filePath}", ex);
                throw;
            }
        }

        /// <summary>
        /// Lädt Zeilen aus einer CSV und fügt sie an die Ziel-Liste an.
        /// Gibt die Anzahl erfolgreich geladener Zeilen zurück.
        /// </summary>
        public int LoadList(string listName, string filePath, char delimiter = ';', bool clearExisting = false)
        {
            EnsureList(listName);
            FileLogger.Info($"Lade Liste gestartet: {listName} <- {filePath}");

            if (!File.Exists(filePath))
            {
                var ex = new FileNotFoundException("CSV-Datei nicht gefunden.", filePath);
                FileLogger.Error($"Laden der Liste fehlgeschlagen: {listName} <- {filePath}", ex);
                throw ex;
            }

            try
            {
                var type = _types[listName];
                var listObj = _listen[listName];

                var props = GetReadableProps(type);
                var propMap = props.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

                // Optional: vorhandene Einträge löschen
                if (clearExisting)
                    InvokeNoArg(listObj, "Clear");

                int loaded = 0;

                using var sr = new StreamReader(filePath, DetectEncoding(filePath));
                string? headerLine = sr.ReadLine();
                if (headerLine == null)
                    return 0;

                var headerCols = SplitCsvLine(headerLine, delimiter);
                var colProps = headerCols.Select(h => propMap.TryGetValue(h, out var prop) ? prop : null).ToArray();

                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    var cols = SplitCsvLine(line, delimiter);
                    if (cols.Length == 0) continue;

                    object instance = Activator.CreateInstance(type)!;

                    for (int i = 0; i < Math.Min(cols.Length, colProps.Length); i++)
                    {
                        var prop = colProps[i];
                        if (prop == null) continue; // unbekannter Header -> ignorieren

                        if (TryParseValue(prop.PropertyType, cols[i], out object? value))
                        {
                            prop.SetValue(instance, value);
                        }
                        else
                        {
                            // Hier könnten Parsing-Fehler gesammelt oder geloggt werden
                        }
                    }

                    // Per Reflection die generische List<T>.Add aufrufen
                    InvokeOneArg(listObj, "Add", instance);
                    loaded++;
                }

                FileLogger.Info($"Laden der Liste erfolgreich: {listName} <- {filePath} (Zeilen={loaded})");
                return loaded;
            }
            catch (Exception ex)
            {
                FileLogger.Error($"Laden der Liste fehlgeschlagen: {listName} <- {filePath}", ex);
                throw;
            }
        }

        // Speichert alle konfigurierten Listen in CSV-Dateien im angegebenen Verzeichnis.
        public void SaveAll(string directoryPath, char delimiter = ';')
        {
            FileLogger.Info($"Speichere alle Listen gestartet -> {directoryPath}");
            try
            {
                Directory.CreateDirectory(directoryPath);

                foreach (var kvp in _listen)
                {
                    string path = Path.Combine(directoryPath, $"{kvp.Key}.csv");
                    SaveList(kvp.Key, path, delimiter);
                }

                FileLogger.Info($"Speichere alle Listen erfolgreich -> {directoryPath}");
            }
            catch (Exception ex)
            {
                FileLogger.Error($"Speichern aller Listen fehlgeschlagen -> {directoryPath}", ex);
                throw;
            }
        }

        // Lädt alle CSV-Dateien aus einem Verzeichnis und fügt die Daten in die jeweiligen Listen ein.
        public int LoadAll(string directoryPath, char delimiter = ';', bool clearExisting = false)
        {
            FileLogger.Info($"Lade alle Listen gestartet <- {directoryPath}");
            int sum = 0;
            try
            {
                foreach (var listName in _listen.Keys)
                {
                    string path = Path.Combine(directoryPath, $"{listName}.csv");
                    if (File.Exists(path))
                        sum += LoadList(listName, path, delimiter, clearExisting);
                }
                FileLogger.Info($"Lade alle Listen erfolgreich <- {directoryPath} (Zeilen={sum})");
                return sum;
            }
            catch (Exception ex)
            {
                FileLogger.Error($"Laden aller Listen fehlgeschlagen <- {directoryPath}", ex);
                throw;
            }
        }

        // ---------- INTERNAL HELPERS ----------

        // Validiert, dass die Liste und ihr Typ konfiguriert sind.
        private void EnsureList(string listName)
        {
            if (!_listen.ContainsKey(listName))
                throw new ArgumentException($"Liste '{listName}' nicht gefunden.", nameof(listName));
            if (!_types.ContainsKey(listName))
                throw new ArgumentException($"Typ für Liste '{listName}' nicht gefunden.", nameof(listName));
        }

        private static IEnumerable<object> Enumerate(object listObj)
        {
            var enumerable = listObj as System.Collections.IEnumerable;
            if (enumerable == null)
                throw new InvalidOperationException("Listenobjekt ist nicht aufzählbar.");
            foreach (var item in enumerable) yield return item!;
        }

        // Liefert öffentlich lesbare, schreibbare Eigenschaften eines Typs (wichtig für Export/Import)
        private static PropertyInfo[] GetReadableProps(Type t) =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
             .Where(p => p.CanRead && p.CanWrite) // writeable ist beim Import nützlich
             .ToArray();

        private static Encoding DetectEncoding(string filePath)
        {
            // Zurzeit wird UTF8 mit BOM verwendet; Erweiterung möglich, um BOM automatisch zu erkennen.
            return new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        }

        private static void InvokeNoArg(object target, string methodName)
        {
            var m = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            m?.Invoke(target, null);
        }

        private static void InvokeOneArg(object target, string methodName, object arg)
        {
            var m = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            m?.Invoke(target, new[] { arg });
        }

        // --- CSV parsing & formatting helper methods ---
        private static string EscapeCsv(string? value, char delimiter)
        {
            if (value == null) value = string.Empty;

            bool mustQuote =
                value.Contains(delimiter) ||
                value.Contains('"') ||
                value.Contains('\n') ||
                value.Contains('\r') ||
                value.StartsWith(" ") ||
                value.EndsWith(" ");

            if (value.Contains('"'))
                value = value.Replace("\"", "\"\""); // doppelte Anführungszeichen in quoted fields

            return mustQuote ? $"\"{value}\"" : value;
        }

        /// <summary>
        /// Splits one CSV line into columns, handling quotes and escaped quotes.
        /// </summary>
        private static string[] SplitCsvLine(string line, char delimiter)
        {
            var cols = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // Check for escaped quote
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i++; // skip next
                        }
                        else
                        {
                            inQuotes = false; // closing quote
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == delimiter)
                    {
                        cols.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            cols.Add(sb.ToString());
            return cols.ToArray();
        }

        // --- Type parsing ---
        private static bool TryParseValue(Type t, string text, out object? value)
        {
            text = text?.Trim() ?? string.Empty;

            if (t == typeof(string))
            {
                value = text;
                return true;
            }

            if (t == typeof(int))
            {
                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int iv))
                {
                    value = iv; return true;
                }
                value = default(int); return false;
            }

            // Extend with more types as needed:
            // if (t == typeof(DateTime)) ...
            // if (t == typeof(decimal)) ...
            // if (t == typeof(bool)) ...

            // Fallback: try ChangeType (works für viele Primitive)
            try
            {
                value = Convert.ChangeType(text, t, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }

}