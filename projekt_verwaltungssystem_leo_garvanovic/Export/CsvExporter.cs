using projekt_verwaltungssystem_leo_garvanovic.Services;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;


namespace projekt_verwaltungssystem_leo_garvanovic.Export
{
    // Leichter CSV-Exporter/Importer. Funktionalität ähnlich wie DataService,
    // aber als separater, fokussierter Helfer genutzt von UI/ExportMenu.
    public sealed class CsvExporter
    {
        private readonly Dictionary<string, object> _listen;
        private readonly Dictionary<string, Type> _types;

        public CsvExporter(Dictionary<string, object> listen, Dictionary<string, Type> listTypes)
        {
            _listen = listen ?? throw new ArgumentNullException(nameof(listen));
            _types = listTypes ?? throw new ArgumentNullException(nameof(listTypes));
        }

        // ---------- PUBLIC API ----------

        // Speichert eine bestimmte Liste als CSV. Standard-Trennzeichen ist ';'.
        public void SaveList(string listName, string filePath, char delimiter = ';', bool includeHeader = true)
        {
            EnsureList(listName);

            var type = _types[listName];
            var listObj = _listen[listName];
            var items = Enumerate(listObj);
            var props = GetReadableProps(type);

            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);

            // UTF-8 mit BOM, damit Excel in de-DE richtig öffnet
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
        }

        /// <summary>
        /// Lädt Zeilen aus CSV und hängt sie an die Ziel-Liste an. Gibt Anzahl geladener Zeilen zurück.
        /// </summary>
        public int LoadList(string listName, string filePath, char delimiter = ';', bool clearExisting = false)
        {
            EnsureList(listName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("CSV-Datei nicht gefunden.", filePath);

            var type = _types[listName];
            var listObj = _listen[listName];

            var props = GetReadableProps(type);
            var propMap = props.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            // Optional: Liste leeren
            InvokeNoArg(listObj, "Clear", onlyIf: clearExisting);

            int loaded = 0;

            using var sr = new StreamReader(filePath, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            string? headerLine = sr.ReadLine();
            if (headerLine == null) return 0;

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
                        // Optional: Parsing-Probleme loggen
                        // FileLogger.Warn($"Konnte '{cols[i]}' nicht für Eigenschaft {prop.Name} parsen");
                    }
                }

                // An List<T> anhängen via Reflection
                InvokeOneArg(listObj, "Add", instance);
                loaded++;
            }

            return loaded;
        }

        public void SaveAll(string directoryPath, char delimiter = ';')
        {
            Directory.CreateDirectory(directoryPath);

            foreach (var kvp in _listen)
            {
                string path = Path.Combine(directoryPath, $"{kvp.Key}.csv");
                SaveList(kvp.Key, path, delimiter);
            }
        }

        public int LoadAll(string directoryPath, char delimiter = ';', bool clearExisting = false)
        {
            int sum = 0;
            foreach (var listName in _listen.Keys)
            {
                string path = Path.Combine(directoryPath, $"{listName}.csv");
                if (File.Exists(path))
                    sum += LoadList(listName, path, delimiter, clearExisting);
            }
            return sum;
        }

        // ---------- INTERNAL HELPERS ----------

        private void EnsureList(string listName)
        {
            if (!_listen.ContainsKey(listName))
                throw new ArgumentException($"Listen '{listName}' nicht gefunden.", nameof(listName));
            if (!_types.ContainsKey(listName))
                throw new ArgumentException($"Typ für Listen '{listName}' nicht gefunden.", nameof(listName));
        }

        private static IEnumerable<object> Enumerate(object listObj)
        {
            var enumerable = listObj as System.Collections.IEnumerable
                ?? throw new InvalidOperationException("Listenobjekt ist nicht aufzählbar.");
            foreach (var item in enumerable) yield return item!;
        }

        private static PropertyInfo[] GetReadableProps(Type t) =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
             .Where(p => p.CanRead && p.CanWrite) // writeable ist beim Import nützlich
             .ToArray();

        private static void InvokeNoArg(object target, string methodName, bool onlyIf)
        {
            if (!onlyIf) return;
            var m = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            m?.Invoke(target, null);
        }

        private static void InvokeOneArg(object target, string methodName, object arg)
        {
            var m = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (m == null)
                throw new MissingMethodException(target.GetType().FullName, methodName);
            m.Invoke(target, new[] { arg });
        }

        // --- CSV formatting ---

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
                        // Escaped quote "" -> a literal "
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

            // Extend here if you add more property types in your models:
            // if (t == typeof(DateTime)) { ... }
            // if (t == typeof(decimal))  { ... }
            // if (t == typeof(bool))     { ... }

            // Fallback: try ChangeType (works for many primitives)
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

