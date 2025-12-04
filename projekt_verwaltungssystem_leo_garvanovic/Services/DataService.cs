using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;


namespace projekt_verwaltungssystem_leo_garvanovic.Services
{
    public sealed class DataService
    {
        
        private readonly Dictionary<string, object> _listen;
        private readonly Dictionary<string, Type> _types;

        public DataService(Dictionary<string, object> listen, Dictionary<string, Type> listTypes)
        {
            _listen = listen ?? throw new ArgumentNullException(nameof(listen));
            _types = listTypes ?? throw new ArgumentNullException(nameof(listTypes));
        }

        public IReadOnlyDictionary<string, object> Lists => _listen;
        public IReadOnlyDictionary<string, Type> Types => _types;

        public List<T> GetList<T>(string name) => (List<T>)_listen[name];
        public Type GetItemType(string name) => _types[name];


        // ---------- PUBLIC API ----------

        public void SaveList(string listName, string filePath, char delimiter = ';', bool includeHeader = true)
        {
            EnsureList(listName);

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
        }

        /// <summary>
        /// Loads rows from CSV and appends to the target list. Returns number of rows successfully loaded.
        /// </summary>
        public int LoadList(string listName, string filePath, char delimiter = ';', bool clearExisting = false)
        {
            EnsureList(listName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("CSV file not found.", filePath);

            var type = _types[listName];
            var listObj = _listen[listName];

            var props = GetReadableProps(type);
            var propMap = props.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            // Optionally clear the list
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
                    if (prop == null) continue; // unknown header -> ignore

                    if (TryParseValue(prop.PropertyType, cols[i], out object? value))
                    {
                        prop.SetValue(instance, value);
                    }
                    else
                    {
                        // You can log/collect errors here if needed
                        // e.g., Console.WriteLine($"Warn: could not parse '{cols[i]}' for {prop.Name}");
                    }
                }

                // Append to List<T> via reflection (works regardless of T)
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
                throw new ArgumentException($"List '{listName}' not found.", nameof(listName));
            if (!_types.ContainsKey(listName))
                throw new ArgumentException($"Type for list '{listName}' not found.", nameof(listName));
        }

        private static IEnumerable<object> Enumerate(object listObj)
        {
            var enumerable = listObj as System.Collections.IEnumerable;
            if (enumerable == null)
                throw new InvalidOperationException("List object is not enumerable.");
            foreach (var item in enumerable) yield return item!;
        }

        private static PropertyInfo[] GetReadableProps(Type t) =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
             .Where(p => p.CanRead && p.CanWrite) // writeable is useful for import
             .ToArray();

        private static Encoding DetectEncoding(string filePath)
        {
            // Use UTF8 by default; if you always write with BOM, UTF8 will be correct.
            // You can extend to detect BOMs if needed.
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

        // --- CSV parsing & formatting ---

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
                value = value.Replace("\"", "\"\""); // double quotes inside quoted fields

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