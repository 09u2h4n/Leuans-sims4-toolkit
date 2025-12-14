using System;
using System.IO;

namespace ModernDesign.Managers
{
    public static class TutorialManager
    {
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string ToolkitFolder = Path.Combine(AppDataPath, "Leuan's - Sims 4 ToolKit");
        private static readonly string TutorialIniPath = Path.Combine(ToolkitFolder, "tutorial.ini");

        public static bool HasCompletedTutorial()
        {
            try
            {
                // Crear carpeta si no existe
                if (!Directory.Exists(ToolkitFolder))
                {
                    Directory.CreateDirectory(ToolkitFolder);
                }

                // Si no existe el archivo, crearlo con false
                if (!File.Exists(TutorialIniPath))
                {
                    CreateTutorialIni(false);
                    return false;
                }

                // Leer el archivo
                var lines = File.ReadAllLines(TutorialIniPath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("hasCompletedTutorial", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = trimmed.Split('=');
                        if (parts.Length == 2)
                        {
                            var value = parts[1].Trim().ToLower();
                            return value == "true";
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void SetTutorialCompleted(bool completed)
        {
            try
            {
                if (!Directory.Exists(ToolkitFolder))
                {
                    Directory.CreateDirectory(ToolkitFolder);
                }

                CreateTutorialIni(completed);
            }
            catch
            {
                // Silently fail
            }
        }

        private static void CreateTutorialIni(bool completed)
        {
            string content = $@"[Tutorial]
hasCompletedTutorial = {completed.ToString().ToLower()}";

            File.WriteAllText(TutorialIniPath, content);
        }
    }
}