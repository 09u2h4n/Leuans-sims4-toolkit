using ModernDesign.MVVM.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModernDesign.Services
{
    public static class SaveGameBackupService
    {
        private static string GetBackupFolder()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string backupFolder = Path.Combine(appData, "Leuan's - Sims 4 ToolKit", "savegame_backups");
            Directory.CreateDirectory(backupFolder);
            return backupFolder;
        }

        /// <summary>
        /// Lee preferences.ini y devuelve un diccionario con las preferencias de cada slot
        /// </summary>
        public static Dictionary<string, SavePreference> LoadPreferences()
        {
            var prefs = new Dictionary<string, SavePreference>(StringComparer.OrdinalIgnoreCase);

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string iniPath = Path.Combine(appData, "Leuan's - Sims 4 ToolKit", "preferences.ini");

            if (!File.Exists(iniPath))
                return prefs;

            string currentSection = string.Empty;

            foreach (var rawLine in File.ReadAllLines(iniPath))
            {
                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Substring(1, line.Length - 2).Trim();
                    continue;
                }

                if (!currentSection.Equals("SaveGames", StringComparison.OrdinalIgnoreCase))
                    continue;

                var parts = line.Split('=');
                if (parts.Length < 2) continue;

                var slotId = parts[0].Trim();
                var value = parts[1].Trim();

                if (Enum.TryParse<SavePreference>(value, ignoreCase: true, out var pref))
                {
                    prefs[slotId] = pref;
                }
            }

            return prefs;
        }

        /// <summary>
        /// Hace backup de un slot específico
        /// </summary>
        public static void BackupSlot(string savesFolder, string slotId)
        {
            if (!Directory.Exists(savesFolder))
                return;

            var files = Directory.GetFiles(savesFolder, $"{slotId}.save*", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
                return;

            string backupFolder = GetBackupFolder();
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string backupFileName = $"{timestamp}_{fileName}";
                string backupPath = Path.Combine(backupFolder, backupFileName);

                try
                {
                    File.Copy(file, backupPath, overwrite: false);
                }
                catch
                {
                    // Si falla un archivo, continuamos con los demás
                }
            }
        }

        /// <summary>
        /// Ejecuta backups según las preferencias configuradas
        /// </summary>
        public static void ExecuteBackupsOnStartup(string savesFolder)
        {
            if (!Directory.Exists(savesFolder))
                return;

            var preferences = LoadPreferences();

            foreach (var kvp in preferences)
            {
                string slotId = kvp.Key;
                SavePreference pref = kvp.Value;

                // Solo hacer backup si está configurado como BackupOnStart
                if (pref == SavePreference.BackupOnStart)
                {
                    BackupSlot(savesFolder, slotId);
                }
            }
        }

        /// <summary>
        /// Limpia backups antiguos (opcional, para no llenar el disco)
        /// Mantiene solo los últimos N backups por slot
        /// </summary>
        public static void CleanOldBackups(int maxBackupsPerSlot = 10)
        {
            string backupFolder = GetBackupFolder();

            if (!Directory.Exists(backupFolder))
                return;

            var allBackups = Directory.GetFiles(backupFolder, "*.save*");

            // Agrupar por slot
            var grouped = allBackups
                .Select(f => new
                {
                    Path = f,
                    FileName = Path.GetFileName(f),
                    // Extraer el slot del nombre: "2025-01-23_14-30-00_Slot_00000001.save"
                    SlotId = ExtractSlotId(Path.GetFileName(f))
                })
                .Where(x => !string.IsNullOrEmpty(x.SlotId))
                .GroupBy(x => x.SlotId);

            foreach (var group in grouped)
            {
                // Ordenar por fecha (más reciente primero)
                var sorted = group.OrderByDescending(x => x.FileName).ToList();

                // Eliminar los que sobran
                for (int i = maxBackupsPerSlot; i < sorted.Count; i++)
                {
                    try
                    {
                        File.Delete(sorted[i].Path);
                    }
                    catch
                    {
                        // Ignorar errores al borrar
                    }
                }
            }
        }

        private static string ExtractSlotId(string fileName)
        {
            // Formato: "2025-01-23_14-30-00_Slot_00000001.save"
            // Buscamos "Slot_" y extraemos hasta el siguiente punto
            int slotIndex = fileName.IndexOf("Slot_", StringComparison.OrdinalIgnoreCase);
            if (slotIndex < 0)
                return null;

            string afterSlot = fileName.Substring(slotIndex);
            int dotIndex = afterSlot.IndexOf(".save");

            if (dotIndex < 0)
                return null;

            return afterSlot.Substring(0, dotIndex + 5); // "Slot_00000001.save"
        }
    }
}