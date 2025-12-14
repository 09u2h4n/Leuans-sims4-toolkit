using System;
using System.IO;

namespace ModernDesign.Localization
{
    public static class LanguageManager
    {
        public static bool IsSpanish { get; private set; }

        static LanguageManager()
        {
            LoadLanguage();
        }

        public static void LoadLanguage()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string iniPath = Path.Combine(appData, "Leuan's - Sims 4 ToolKit", "language.ini");

                // Si no existe, por defecto inglés
                if (!File.Exists(iniPath))
                {
                    IsSpanish = false;
                    return;
                }

                var lines = File.ReadAllLines(iniPath);
                foreach (var raw in lines)
                {
                    var line = raw.Trim();
                    if (line.StartsWith("Language", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split('=');
                        if (parts.Length >= 2)
                        {
                            var value = parts[1].Trim();
                            // Todo lo que empiece con "es" lo tratamos como español
                            IsSpanish = value.StartsWith("es", StringComparison.OrdinalIgnoreCase);
                            return;
                        }
                    }
                }

                // Si no se encontró la línea, inglés
                IsSpanish = false;
            }
            catch
            {
                // En caso de error leyendo el archivo: default inglés
                IsSpanish = false;
            }
        }
    }
}
