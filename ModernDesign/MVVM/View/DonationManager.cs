using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModernDesign.Managers
{
    public static class DonationManager
    {
        private static readonly string AppDataRoaming = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Leuan's - Sims 4 ToolKit"
        );

        private static readonly string AppDataLocal = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LTK"
        );

        private static readonly string ProfileIniPath = Path.Combine(AppDataRoaming, "profile.ini");
        private static readonly string TmpFile2025Path = Path.Combine(AppDataLocal, "tmpFile2025.ini");

        // Activar donador con una key del servidor
        public static async Task<bool> ActivateDonorAsync()
        {
            try
            {
                // Crear carpetas si no existen
                if (!Directory.Exists(AppDataRoaming))
                    Directory.CreateDirectory(AppDataRoaming);

                if (!Directory.Exists(AppDataLocal))
                    Directory.CreateDirectory(AppDataLocal);

                // Obtener key válida del servidor
                string validKey = await GetValidKeyFromServerAsync();
                if (string.IsNullOrEmpty(validKey))
                    return false;

                // Crear tmpFile2025.ini con la key
                using (StreamWriter writer = new StreamWriter(TmpFile2025Path))
                {
                    writer.WriteLine($"key={validKey}");
                }

                // Crear/actualizar profile.ini con isPatreonSupporter=true y key
                UpdateProfileIni(validKey);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Activar donador manualmente con una key específica
        public static bool ActivateDonorWithKey(string key)
        {
            try
            {
                // Crear carpetas si no existen
                if (!Directory.Exists(AppDataRoaming))
                    Directory.CreateDirectory(AppDataRoaming);

                if (!Directory.Exists(AppDataLocal))
                    Directory.CreateDirectory(AppDataLocal);

                // Crear tmpFile2025.ini con la key
                using (StreamWriter writer = new StreamWriter(TmpFile2025Path))
                {
                    writer.WriteLine($"key={key}");
                }

                // Crear/actualizar profile.ini con isPatreonSupporter=true y key
                UpdateProfileIni(key);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void UpdateProfileIni(string key)
        {
            try
            {
                var lines = File.Exists(ProfileIniPath) ? File.ReadAllLines(ProfileIniPath) : new string[0];

                using (StreamWriter writer = new StreamWriter(ProfileIniPath))
                {
                    bool patreonWritten = false;
                    bool keyWritten = false;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("isPatreonSupporter="))
                        {
                            writer.WriteLine("isPatreonSupporter=true");
                            patreonWritten = true;
                        }
                        else if (line.StartsWith("key="))
                        {
                            writer.WriteLine($"key={key}");
                            keyWritten = true;
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                    }

                    // Si no existían, agregarlos
                    if (!patreonWritten)
                        writer.WriteLine("isPatreonSupporter=true");
                    if (!keyWritten)
                        writer.WriteLine($"key={key}");
                }
            }
            catch { }
        }

        private static async Task<string> GetValidKeyFromServerAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    string response = await client.GetStringAsync("https://zeroauno.blob.core.windows.net/leuan/Public/version.txt?sp=r&st=2025-12-29T23:46:08Z&se=2026-02-28T08:01:08Z&spr=https&sv=2024-11-04&sr=b&sig=vuJZkucCfCl6oWLB0PPCx3vwW58yS%2FtdhHjOsCavOOM%3D");
                    return response.Trim();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}