using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModernDesign.Managers
{
    public class AnnouncementData
    {
        public bool IsEnabled { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string LogoUrl { get; set; }
    }

    public static class AnnouncementManager
    {
        private const string ANNOUNCEMENT_URL = "https://zeroauno.blob.core.windows.net/leuan/Public/Anuncio.txt?sp=r&st=2025-12-29T23:44:34Z&se=2026-02-28T07:59:34Z&spr=https&sv=2024-11-04&sr=b&sig=5LICZsBvJWw63%2BZzqPMwg3dgtIZTxdGKX3vwuDc0TO8%3D";

        public static async Task<AnnouncementData> GetAnnouncementAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    string content = await client.GetStringAsync(ANNOUNCEMENT_URL);

                    // Parsear el contenido
                    var data = new AnnouncementData
                    {
                        IsEnabled = false,
                        Text = string.Empty,
                        ImageUrl = string.Empty,
                        LogoUrl = string.Empty
                    };

                    string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("enableAnnounce", StringComparison.OrdinalIgnoreCase))
                        {
                            string value = line.Substring(line.IndexOf('=') + 1).Trim();
                            data.IsEnabled = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                        }
                        else if (line.StartsWith("announceText", StringComparison.OrdinalIgnoreCase))
                        {
                            int equalsIndex = line.IndexOf('=');
                            if (equalsIndex >= 0 && equalsIndex + 1 < line.Length)
                            {
                                data.Text = line.Substring(equalsIndex + 1).Trim().Trim('"');
                            }
                        }
                        else if (line.StartsWith("imageURL", StringComparison.OrdinalIgnoreCase))
                        {
                            int equalsIndex = line.IndexOf('=');
                            if (equalsIndex >= 0 && equalsIndex + 1 < line.Length)
                            {
                                data.ImageUrl = line.Substring(equalsIndex + 1).Trim().Trim('"');
                            }
                        }
                        else if (line.StartsWith("logoURL", StringComparison.OrdinalIgnoreCase))
                        {
                            int equalsIndex = line.IndexOf('=');
                            if (equalsIndex >= 0 && equalsIndex + 1 < line.Length)
                            {
                                data.LogoUrl = line.Substring(equalsIndex + 1).Trim().Trim('"');
                            }
                        }
                    }

                    return data;
                }
            }
            catch (Exception)
            {
                // Si falla la descarga o parseo, retornamos datos vacíos
                return new AnnouncementData
                {
                    IsEnabled = false,
                    Text = string.Empty,
                    ImageUrl = string.Empty,
                    LogoUrl = string.Empty
                };
            }
        }
    }
}