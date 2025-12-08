using projekt_verwaltungssystem_leo_garvanovic.Models;

namespace projekt_verwaltungssystem_leo_garvanovic.UI
{
    // Thin facade kept for compatibility with existing callers.
    public static class Menu
    {
        public static void ShowMainMenu(Benutzer user) => MainMenu.Show(user);

        // Existing code referenced some internal helpers previously; keep forwarding methods
        // if other parts of the codebase call them directly.
        public static void ShowListenMenu(Benutzer user) => ListMenu.ShowListenMenu(user);
        public static void ExportFunktionen() => ExportMenu.ExportFunktionen();
    }
}
