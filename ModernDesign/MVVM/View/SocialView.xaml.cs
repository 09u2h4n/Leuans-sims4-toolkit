using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernDesign.MVVM.View
{
    public partial class SocialView : UserControl
    {
        private string _languageCode = "en-US";

        // ===== CONFIGURA TUS ENLACES AQUÍ =====
        private const string WEBSITE_URL = "https://leuan.zeroauno.com/sims4-toolkit/index.html";
        private const string DISCORD_SERVER_URL = "https://discord.gg/JYnpPt4nUu";
        private const string PERSONAL_DISCORD = "leuan"; // Tu usuario de Discord
        // ========================================

        public SocialView()
        {
            InitializeComponent();
            InitLocalization();
        }

        private void InitLocalization()
        {
            LoadLanguageFromIni();
            bool es = _languageCode.StartsWith("es", StringComparison.OrdinalIgnoreCase);

            // Header
            TitleText.Text = es ? "Conéctate con Nosotros" : "Connect with Us";
            SubtitleText.Text = es ? "Únete a nuestra comunidad y mantente actualizado con las últimas noticias"
                                   : "Join our community and stay updated with the latest news";

            // Cards
            WebsiteTitle.Text = es ? "Sitio Web" : "Website";
            DiscordServerTitle.Text = es ? "Servidor de Discord" : "Discord Server";
            DiscordServerDesc.Text = es ? "Únete a la comunidad" : "Join the community";
            PersonalDiscordTitle.Text = es ? "Discord Personal" : "Personal Discord";
            PersonalDiscordDesc.Text = PERSONAL_DISCORD;

            // Thanks section
            ThanksTitle.Text = es ? "Agradecimientos Especiales" : "Special Thanks";

            AnadiusThanks.Text = es
                ? "Por todo el increíble trabajo en los desbloqueadores de DLC. Descansa en paz. 🕊️"
                : "For all the incredible work on DLC unlockers. Rest in peace. 🕊️";

            CommunityThanks.Text = es
                ? "Por mantener viva y próspera la escena del modding."
                : "For keeping the modding scene alive and thriving.";

            CreatorsThanks.Text = es
                ? "Tu creatividad hace Los Sims 4 infinitamente mejor."
                : "Your creativity makes The Sims 4 infinitely better.";

            TestersThanks.Text = es
                ? "Por encontrar bugs y sugerir mejoras."
                : "For finding bugs and suggesting improvements.";

            DiscordThanks.Text = es
                ? "Por ser una comunidad increíble y solidaria."
                : "For being an amazing and supportive community.";

            YouTitle.Text = es ? "¡Tú! 💜" : "You! 💜";
            YouThanks.Text = es
                ? "Por usar esta herramienta y apoyar el proyecto."
                : "For using this tool and supporting the project.";

            FooterText.Text = es ? "Hecho con 💜 por Leuan" : "Made with 💜 by Leuan";
        }

        private void LoadLanguageFromIni()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string iniPath = Path.Combine(appData, "Leuan's - Sims 4 ToolKit", "language.ini");

                if (!File.Exists(iniPath)) return;

                foreach (var line in File.ReadAllLines(iniPath))
                {
                    if (line.Trim().StartsWith("Language", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split('=');
                        if (parts.Length >= 2)
                            _languageCode = parts[1].Trim();
                        break;
                    }
                }

                if (_languageCode != "es-ES" && _languageCode != "en-US")
                    _languageCode = "en-US";
            }
            catch { _languageCode = "en-US"; }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                bool es = _languageCode.StartsWith("es", StringComparison.OrdinalIgnoreCase);
                MessageBox.Show(
                    es ? $"No se pudo abrir el enlace:\n{url}\n\nError: {ex.Message}"
                       : $"Could not open link:\n{url}\n\nError: {ex.Message}",
                    es ? "Error" : "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void WebsiteCard_Click(object sender, MouseButtonEventArgs e)
        {
            OpenUrl(WEBSITE_URL);
        }

        private void DiscordServerCard_Click(object sender, MouseButtonEventArgs e)
        {
            OpenUrl(DISCORD_SERVER_URL);
        }

        private void PersonalDiscordCard_Click(object sender, MouseButtonEventArgs e)
        {
            bool es = _languageCode.StartsWith("es", StringComparison.OrdinalIgnoreCase);

            try
            {
                Clipboard.SetText(PERSONAL_DISCORD);

                MessageBox.Show(
                    es ? $"'{PERSONAL_DISCORD}' copiado al portapapeles.\n\n✅ Pégalo en Discord para agregarme como amigo."
                       : $"'{PERSONAL_DISCORD}' copied to clipboard.\n\n✅ Paste it in Discord to add me as a friend.",
                    es ? "✓ Copiado" : "✓ Copied",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    es ? $"❌ No se pudo copiar al portapapeles.\n\nMi Discord es: {PERSONAL_DISCORD}\n\nError: {ex.Message}"
                       : $"❌ Could not copy to clipboard.\n\nMy Discord is: {PERSONAL_DISCORD}\n\nError: {ex.Message}",
                    es ? "Error" : "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
}