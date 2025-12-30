using ModernDesign.Profile;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernDesign
{
    public partial class LanguageSelector : Window
    {
        private readonly string _languageIniPath;

        public LanguageSelector()
        {
            InitializeComponent();
            _languageIniPath = GetLanguageIniPath();

            // Set default selection
            rbEnglish.IsChecked = true;
        }

        private string GetLanguageIniPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string toolkitFolder = Path.Combine(appData, "Leuan's - Sims 4 ToolKit");
            return Path.Combine(toolkitFolder, "language.ini");
        }

        private void TxtUserName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            bool hasLanguage = rbSpanish.IsChecked == true || rbEnglish.IsChecked == true;

            btnContinue.IsEnabled = !string.IsNullOrWhiteSpace(userName) && hasLanguage;

            if (string.IsNullOrWhiteSpace(userName))
            {
                txtNameHint.Text = rbSpanish.IsChecked == true
                    ? "Ingresa tu nombre o apodo"
                    : "Enter your name or nickname";
                txtNameHint.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#64748B"));
            }
            else
            {
                txtNameHint.Text = rbSpanish.IsChecked == true
                    ? $"✓ Perfecto, {userName}!"
                    : $"✓ Perfect, {userName}!";
                txtNameHint.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#22C55E"));
            }
        }

        private async void Continue_Click(object sender, RoutedEventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            string languageCode = null;

            if (rbSpanish.IsChecked == true)
                languageCode = "es-ES";
            else if (rbEnglish.IsChecked == true)
                languageCode = "en-US";

            // Validations
            if (string.IsNullOrWhiteSpace(userName))
            {
                MessageBox.Show(
                    rbSpanish.IsChecked == true
                        ? "Por favor ingresa tu nombre."
                        : "Please enter your name.",
                    "Name Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if (languageCode == null)
            {
                MessageBox.Show(
                    "Por favor selecciona un idioma / Please select a language.",
                    "Language Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            try
            {
                // Create Profile
                ProfileManager.CreateProfile(userName);

                // Save Language Configuration
                SaveLanguageConfig(languageCode);

                // ✅ SEND DISCORD WEBHOOK
                //await SendDiscordWebhook(userName, languageCode);

                // ✅ SHOW RESTART MESSAGE
                string message = rbSpanish.IsChecked == true
                    ? "Configuración guardada exitosamente.\n\n" +
                      "La aplicación se cerrará ahora.\n" +
                      "Por favor, abre el programa nuevamente para aplicar los cambios."
                    : "Configuration saved successfully.\n\n" +
                      "The application will now close.\n" +
                      "Please open the program again to apply the changes.";

                string title = rbSpanish.IsChecked == true
                    ? "Reinicio Requerido"
                    : "Restart Required";

                MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // ✅ OPEN DISCORD INVITE
                OpenDiscordInvite();

                // ✅ CLOSE THE APPLICATION
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating profile:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

       // No more telemetry.

        private void OpenDiscordInvite()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "https://discord.gg/JYnpPt4nUu",
                    UseShellExecute = false
                });
            }
            catch
            {
                // Silently fail - don't interrupt user experience if browser fails to open
            }
        }

        private void SaveLanguageConfig(string languageCode)
        {
            string directory = Path.GetDirectoryName(_languageIniPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string[] lines = new string[]
            {
                "# ------------------------------------ Leuan's - Sims 4 ToolKit ------------------------------------",
                "# Website: leuan.zeroauno.com",
                "# Discord: leuan",
                " ",
                "# Available Languages: es-ES | en-US",
                "# More languages will be added soon.",
                " ",
                "# Lenguajes Disponibles: es-ES | en-US",
                "# Más lenguajes serán agregados pronto.",
                " ",
                "\"Digital Culture was born to be shared,",
                "but the big companies locked it away.",
                "Piracy is just the human gesture",
                "of ensuring that culture remains accessible to everyone,",
                "and not only to those who can afford the luxury of paying.\"",
                " ",
                "\"La cultura digital nació para compartirse,",
                "pero las grandes compañías lo privatizaron.",
                "La piratería es solo un gesto humano que se asegura que esta cultura sea accesible para todos,",
                "y no solo para aquellos que tienen el lujo de poder pagar.\"",
                "# ------------------------------------ Leuan's - Sims 4 ToolKit ------------------------------------",
                " ",
                "[General]",
                "Language = " + languageCode,
            };

            File.WriteAllLines(_languageIniPath, lines);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}