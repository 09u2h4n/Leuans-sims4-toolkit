using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ModernDesign.Profile;

namespace ModernDesign.Managers
{
    public static class DeveloperModeManager
    {
        private static readonly string AppDataRoaming = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Leuan's - Sims 4 ToolKit"
        );

        private static readonly string AppDataLocal = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LTK"
        );

        private static readonly string ProgressFilePath = Path.Combine(AppDataRoaming, "progress.ini");
        private static readonly string SettingsPFilePath = Path.Combine(AppDataRoaming, "settingsp.ini");
        private static readonly string ProfileIniPath = Path.Combine(AppDataRoaming, "profile.ini");
        private static readonly string TmpFile2025Path = Path.Combine(AppDataLocal, "tmpFile2025.ini");

        // Lista de features que deben ser visitadas
        private static readonly string[] RequiredFeatures = new string[]
        {
            "install_mods",
            "mod_manager",
            "loading_screen",
            "cheats_guide",
            "gallery_manager",
            "music_manager",
            "gameplay_enhancer",
            "fix_common_errors",
            "method_5050"
        };

        static DeveloperModeManager()
        {
            // Crear carpetas si no existen
            if (!Directory.Exists(AppDataRoaming))
                Directory.CreateDirectory(AppDataRoaming);

            if (!Directory.Exists(AppDataLocal))
                Directory.CreateDirectory(AppDataLocal);

            // Crear archivo de progreso si no existe
            if (!File.Exists(ProgressFilePath))
            {
                using (StreamWriter writer = new StreamWriter(ProgressFilePath))
                {
                    foreach (var feature in RequiredFeatures)
                    {
                        writer.WriteLine($"{feature}=false");
                    }
                }
            }
        }

        // Marcar una feature como visitada
        public static void MarkFeatureAsVisited(string featureId)
        {
            try
            {
                var lines = File.ReadAllLines(ProgressFilePath);
                using (StreamWriter writer = new StreamWriter(ProgressFilePath))
                {
                    bool found = false;
                    foreach (var line in lines)
                    {
                        if (line.StartsWith($"{featureId}="))
                        {
                            writer.WriteLine($"{featureId}=true");
                            found = true;
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                    }

                    // Si no existía, agregarlo
                    if (!found)
                    {
                        writer.WriteLine($"{featureId}=true");
                    }
                }
            }
            catch { }
        }

        // Verificar si una feature fue visitada
        public static bool IsFeatureVisited(string featureId)
        {
            try
            {
                if (!File.Exists(ProgressFilePath))
                    return false;

                foreach (var line in File.ReadAllLines(ProgressFilePath))
                {
                    if (line.StartsWith($"{featureId}="))
                    {
                        return line.EndsWith("=true");
                    }
                }
            }
            catch { }

            return false;
        }

        // Verificar si todas las features fueron visitadas
        public static bool AreAllFeaturesVisited()
        {
            foreach (var feature in RequiredFeatures)
            {
                if (!IsFeatureVisited(feature))
                    return false;
            }
            return true;
        }

        // Verificar si tiene todas las medallas de oro
        public static bool HasAllGoldMedals()
        {
            string[] tutorialIds = new string[]
            {
                "beginner_guide",
                "tutorial_trait",
                "tutorial_interaction",
                "tutorial_career",
                "tutorial_buff",
                "tutorial_clothing",
                "tutorial_object"
            };

            foreach (var tutorialId in tutorialIds)
            {
                if (ProfileManager.GetTutorialMedal(tutorialId) != MedalType.Gold)
                    return false;
            }

            return true;
        }

        // Verificar si ha donado (NUEVA LÓGICA CON 3 REQUISITOS)
        public static async Task<bool> HasDonatedAsync()
        {
            try
            {
                // REQUISITO 1: Verificar que exista settingsp.ini
                if (!File.Exists(SettingsPFilePath))
                    return false;

                // REQUISITO 2: Verificar tmpFile2025.ini con key válida
                if (!File.Exists(TmpFile2025Path))
                    return false;

                string tmpFileKey = null;
                foreach (var line in File.ReadAllLines(TmpFile2025Path))
                {
                    if (line.StartsWith("key="))
                    {
                        tmpFileKey = line.Substring("key=".Length).Trim();
                        break;
                    }
                }

                if (string.IsNullOrEmpty(tmpFileKey))
                    return false;

                // REQUISITO 3: Verificar profile.ini con isPatreonSupporter=true y key válida
                if (!File.Exists(ProfileIniPath))
                    return false;

                bool isPatreonSupporter = false;
                string profileKey = null;

                foreach (var line in File.ReadAllLines(ProfileIniPath))
                {
                    if (line.StartsWith("isPatreonSupporter="))
                    {
                        string value = line.Substring("isPatreonSupporter=".Length).Trim();
                        isPatreonSupporter = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                    }
                    else if (line.StartsWith("key="))
                    {
                        profileKey = line.Substring("key=".Length).Trim();
                    }
                }

                if (!isPatreonSupporter || string.IsNullOrEmpty(profileKey))
                    return false;

                // Obtener la key válida desde el servidor
                string validKey = await GetValidKeyFromServerAsync();
                if (string.IsNullOrEmpty(validKey))
                    return false;

                // Verificar que AMBAS keys coincidan con la del servidor
                bool tmpFileValid = tmpFileKey.Equals(validKey, StringComparison.OrdinalIgnoreCase);
                bool profileValid = profileKey.Equals(validKey, StringComparison.OrdinalIgnoreCase);

                return tmpFileValid && profileValid;
            }
            catch
            {
                return false;
            }
        }

        // Verificación síncrona (usa cache)
        private static bool? _donationStatusCache = null;
        private static DateTime _lastCheckTime = DateTime.MinValue;

        public static bool HasDonated()
        {
            try
            {
                if (!File.Exists(SettingsPFilePath))
                    return false;

                if (!File.Exists(TmpFile2025Path))
                    return false;

                string tmpFileKey = null;
                foreach (var line in File.ReadAllLines(TmpFile2025Path))
                {
                    if (line.StartsWith("key="))
                    {
                        tmpFileKey = line.Substring("key=".Length).Trim();
                        break;
                    }
                }

                if (string.IsNullOrEmpty(tmpFileKey))
                    return false;

                if (!File.Exists(ProfileIniPath))
                    return false;

                bool isPatreonSupporter = false;
                string profileKey = null;

                foreach (var line in File.ReadAllLines(ProfileIniPath))
                {
                    if (line.StartsWith("isPatreonSupporter="))
                    {
                        string value = line.Substring("isPatreonSupporter=".Length).Trim();
                        isPatreonSupporter = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                    }
                    else if (line.StartsWith("key="))
                    {
                        profileKey = line.Substring("key=".Length).Trim();
                    }
                }

                if (!isPatreonSupporter || string.IsNullOrEmpty(profileKey))
                    return false;

                // Verificar que AMBAS keys sean iguales entre sí
                // (No verificamos con el servidor en modo síncrono para evitar lag)
                bool keysMatch = tmpFileKey.Equals(profileKey, StringComparison.OrdinalIgnoreCase);

                return keysMatch;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<string> GetValidKeyFromServerAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    string response = await client.GetStringAsync("https://zeroauno.blob.core.windows.net/leuan/Public/version.txt?sp=r&st=2025-12-29T23:46:08Z&se=2026-02-28T08:01:08Z&spr=https&sv=2024-11-04&sr=b&sig=vuJZkucCfCl6oWLB0PPCx3vwW58yS%2FtdhHjOsCavOOM%3D");
                    return response.Trim();
                }
            }
            catch
            {
                return null;
            }
        }

        // Verificar si el Developer Mode está desbloqueado
        public static bool IsDeveloperModeUnlocked()
        {
            return HasAllGoldMedals() && AreAllFeaturesVisited() && HasDonated();
        }

        // Obtener progreso detallado
        public static DeveloperModeProgress GetProgress()
        {
            return new DeveloperModeProgress
            {
                HasAllGoldMedals = HasAllGoldMedals(),
                AllFeaturesVisited = AreAllFeaturesVisited(),
                HasDonated = HasDonated(),
                FeaturesVisited = GetVisitedFeaturesCount(),
                TotalFeatures = RequiredFeatures.Length
            };
        }

        private static int GetVisitedFeaturesCount()
        {
            int count = 0;
            foreach (var feature in RequiredFeatures)
            {
                if (IsFeatureVisited(feature))
                    count++;
            }
            return count;
        }
    }

    public class DeveloperModeProgress
    {
        public bool HasAllGoldMedals { get; set; }
        public bool AllFeaturesVisited { get; set; }
        public bool HasDonated { get; set; }
        public int FeaturesVisited { get; set; }
        public int TotalFeatures { get; set; }

        public bool IsUnlocked => HasAllGoldMedals && AllFeaturesVisited && HasDonated;
        public int ProgressPercentage => (int)((
            (HasAllGoldMedals ? 33.33 : 0) +
            (AllFeaturesVisited ? 33.33 : 0) +
            (HasDonated ? 33.33 : 0)
        ));
    }
}