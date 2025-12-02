# 📄 Verwaltungssoftware (Konsolenanwendung in C#)

## 1. Projektbeschreibung
Die Verwaltungssoftware ist eine Konsolenanwendung, die grundlegende Verwaltungsfunktionen für verschiedene Datenobjekte (Personen, Autos, Handys, Computer) bereitstellt. Sie ermöglicht die Anmeldung mit unterschiedlichen Benutzerrollen (Admin und User), die Verwaltung von Listen, Such- und Filterfunktionen sowie den Export von Daten.  
**Zusätzlich wird GitHub für Versionskontrolle und tägliche Commits genutzt, um den Projektfortschritt transparent zu dokumentieren.**

---

## 2. Ziel des Projekts
- Entwicklung einer modularen, leicht erweiterbaren Verwaltungssoftware.
- Bereitstellung einer Benutzerverwaltung mit Rollen.
- Implementierung von Datenpersistenz (Datei oder Datenbank).
- Exportfunktionen (CSV, optional PDF).
- Dokumentation aller Schritte und Funktionen.
- **Nachweis des Projektfortschritts durch tägliche GitHub-Commits.**

---

## 3. Anforderungen
### Funktionale Anforderungen
- Login-System mit Benutzerrollen (Admin/User).
- Menüführung mit Untermenüs.
- CRUD-Operationen für Listen.
- Such- und Filterfunktionen.
- Datenpersistenz (Dateien oder Datenbank).
- Exportfunktionen (CSV, optional PDF).

### Nicht-funktionale Anforderungen
- Konsolenbasierte Benutzeroberfläche.
- Deutsche Sprache für Interface und Dokumentation.
- Gute Lesbarkeit und Strukturierung des Codes.
- **Versionskontrolle mit GitHub (tägliche Commits).**

---

## 4. Architektur
### Projektstruktur
```
/Models
    Benutzer.cs
    Person.cs
    Auto.cs
    Handy.cs
    Computer.cs
/Services
    AuthService.cs
    DataService.cs
/UI
    Menu.cs
/Export
    CsvExporter.cs
Program.cs
```

### Klassendiagramm (vereinfacht)
```
Benutzer
+ Username : string
+ Password : string
+ Role : string

Person
+ Id : int
+ Name : string
+ Alter : int
```

---

## 5. Implementierungsschritte
### Phase 1: Planung
- Erstellung des Pflichtenhefts.
- Definition der Klassen und Methoden.
- Skizzierung des Menü-Flows.
- **Initialer GitHub-Commit mit Projektstruktur.**

### Phase 2: Login-System
- Implementierung der Klasse `Benutzer`.
- Authentifizierung über `AuthService`.
- Unterschiedliche Menüs je nach Rolle.
- **Commit: Login-Funktionalität.**

### Phase 3: Menüführung
- Hauptmenü mit Optionen.
- Untermenüs für Benutzer- und Listenverwaltung.
- **Commit: Menü-Logik.**

### Phase 4: Datenmodelle & CRUD
- Erstellung der Modelle (`Person`, `Auto`, etc.).
- Implementierung von Anlegen, Bearbeiten, Löschen.
- Such- und Filterfunktionen.
- **Commit: CRUD-Funktionen für erste Liste.**

### Phase 5: Benutzerverwaltung
- Admin-Funktionen: Benutzer hinzufügen/löschen.
- Speichern und Laden von Benutzerdaten.
- **Commit: Benutzerverwaltung abgeschlossen.**

### Phase 6: Export
- CSV-Export für Listen.
- Optional: PDF-Export.
- **Commit: Exportfunktionen.**

### Phase 7: Feinschliff
- Code-Optimierung.
- Kommentare und Dokumentation.
- Screenshots und Testfälle.
- **Commit: Finalisierung und Dokumentation.**

---

## 6. GitHub-Workflow
- **Repository erstellen**: `Verwaltungssoftware-CSharp`.
- **Branch-Strategie**:
  - `main` für stabile Versionen.
  - `dev` für Entwicklung.
- **Tägliche Commits**:
  - Jeder Arbeitsschritt wird mit einem Commit dokumentiert.
  - Commit-Nachrichten in Deutsch, z. B.:
    - `feat: Login-System implementiert`
    - `fix: Fehler bei Menüauswahl behoben`
    - `docs: Dokumentation aktualisiert`
- **Push am Ende des Arbeitstags**:
  - Alle Änderungen hochladen.
  - Optional: GitHub Issues für Aufgabenverwaltung.

---

## 7. Testplan
- Login-Test.
- Menü-Test.
- CRUD-Test.
- Persistenz-Test.
- Export-Test.

---

## 8. Beispielcode
### Login-System
```csharp
public class Benutzer
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public Benutzer(string username, string password, string role)
    {
        Username = username;
        Password = password;
        Role = role;
    }
}
```

### Authentifizierung
```csharp
public class AuthService
{
    private List<Benutzer> benutzerListe = new List<Benutzer>
    {
        new Benutzer("admin", "1234", "Admin"),
        new Benutzer("user", "pass", "User")
    };

    public Benutzer Login()
    {
        Console.Write("Benutzername: ");
        string username = Console.ReadLine();
        Console.Write("Passwort: ");
        string password = Console.ReadLine();

        return benutzerListe.FirstOrDefault(b => b.Username == username && b.Password == password);
    }
}
```

### Menü
```csharp
public static class Menu
{
    public static void ShowMainMenu(Benutzer user)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("
--- Hauptmenü ---");
            Console.WriteLine("1. Benutzerverwaltung");
            Console.WriteLine("2. Listenverwaltung");
            Console.WriteLine("3. Export");
            Console.WriteLine("4. Logout");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    if (user.Role == "Admin") BenutzerVerwaltung();
                    else Console.WriteLine("Keine Berechtigung!");
                    break;
                case "2":
                    ListenVerwaltung();
                    break;
                case "3":
                    ExportFunktionen();
                    break;
                case "4":
                    running = false;
                    break;
            }
        }
    }

    private static void BenutzerVerwaltung() => Console.WriteLine("Benutzerverwaltung...");
    private static void ListenVerwaltung() => Console.WriteLine("Listenverwaltung...");
    private static void ExportFunktionen() => Console.WriteLine("Exportfunktionen...");
}
```

---

**Ende der Dokumentation**
