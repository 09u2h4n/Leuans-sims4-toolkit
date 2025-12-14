using ModernDesign.MVVM.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ModernDesign
{
    public partial class InstallModeSelector : Window
    {
        public InstallModeSelector()
        {
            InitializeComponent();

            // Cargar links por defecto primero
            SetDefaultLinks();

            // Cargar links dinámicamente
            Loaded += async (s, e) => await LoadTutorialLinksAsync();

            ApplyLanguage();

            this.MouseLeftButtonDown += Window_MouseLeftButtonDown;
        }

        // ✅ DICCIONARIO PARA ALMACENAR LOS LINKS
        private Dictionary<string, string> _tutorialLinks = new Dictionary<string, string>();
        private readonly HttpClient _httpClient = new HttpClient();

        // ✅ MÉTODO PARA CARGAR LOS LINKS DESDE EL ARCHIVO REMOTO
        private async Task LoadTutorialLinksAsync()
        {
            try
            {
                string url = "https://zeroauno.blob.core.windows.net/leuan/TheSims4/Utility/links.txt";
                string content = await _httpClient.GetStringAsync(url);

                // Parsear el contenido
                var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.Contains("="))
                    {
                        var parts = trimmed.Split(new[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            _tutorialLinks[key] = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si falla la carga, usar URLs por defecto
                System.Diagnostics.Debug.WriteLine($"Error loading tutorial links: {ex.Message}");
                SetDefaultLinks();
            }
        }

        // ✅ MÉTODO PARA ESTABLECER LINKS POR DEFECTO (FALLBACK)
        private void SetDefaultLinks()
        {
            _tutorialLinks["tutorialAutomatico"] = "https://youtu.be/GeTuyL89JOM?si=siu_WW92ecFKF-df&t=72s";
            _tutorialLinks["tutorialManual"] = "https://www.youtube.com/watch?v=TF0EBobPWdc";
            _tutorialLinks["legitInstall"] = "https://www.youtube.com/watch?v=YOUR_VIDEO_ID_HERE";
            _tutorialLinks["manualInstall"] = "https://www.youtube.com/watch?v=YOUR_VIDEO_ID_HERE";
            _tutorialLinks["manualInstall2"] = "https://www.youtube.com/watch?v=YOUR_VIDEO_ID_HERE";
        }

        // ✅ MÉTODO PARA OBTENER UN LINK (CON FALLBACK)
        private string GetTutorialLink(string key, string defaultUrl = "")
        {
            if (_tutorialLinks.ContainsKey(key))
            {
                return _tutorialLinks[key];
            }
            return defaultUrl;
        }

        private void ApplyLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                TitleText.Text = "Elige Tu Método";
                SubtitleText.Text = "Selecciona cómo deseas instalar los DLC's";

                // Automatic
                AutomaticTitle.Text = "Automático";
                AutomaticDesc.Text = "Déjanos encargarnos de todo por ti";

                // Manual
                ManualTitle.Text = "Manual";
                ManualDesc.Text = "Control completo sobre cada detalle";
            }
            else
            {
                TitleText.Text = "Choose Your Method";
                SubtitleText.Text = "Select how you want to install the DLC's";

                // Automatic
                AutomaticTitle.Text = "Automatic";
                AutomaticDesc.Text = "Let us handle everything for you";

                // Manual
                ManualTitle.Text = "Manual";
                ManualDesc.Text = "Complete control over every detail, manual download, manual installation, but we'll guide you through.";
            }
        }

        private static bool IsSpanishLanguage()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string languagePath = Path.Combine(appData, "Leuan's - Sims 4 ToolKit", "language.ini");

                if (!File.Exists(languagePath))
                    return false;

                var lines = File.ReadAllLines(languagePath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("Language") && trimmed.Contains("="))
                    {
                        var parts = trimmed.Split('=');
                        if (parts.Length == 2)
                        {
                            return parts[1].Trim().ToLower().Contains("es");
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

        private void AutomaticBtn_Click(object sender, MouseButtonEventArgs e)
        {
            string tutorialUrl = GetTutorialLink("tutorialAutomatico", "https://youtu.be/GeTuyL89JOM?si=siu_WW92ecFKF-df&t=72s");
            ShowTutorialPrompt(tutorialUrl, new UpdaterWindow());
        }

        private void SemiAutomaticBtn_Click(object sender, MouseButtonEventArgs e)
        {
            string tutorialUrl = GetTutorialLink("tutorialManual", "https://www.youtube.com/watch?v=TF0EBobPWdc");
            ShowTutorialPrompt(tutorialUrl, new SemiAutoInstallerWindow());
        }

        private void ShowTutorialPrompt(string tutorialUrl, Window targetWindow)
        {
            bool isSpanish = IsSpanishLanguage();

            string message = isSpanish
                ? "¿Te gustaría ver el tutorial?"
                : "Would you like to see the tutorial?";

            string caption = isSpanish
                ? "Tutorial"
                : "Tutorial";

            MessageBoxResult result = MessageBox.Show(
                message,
                caption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // ✅ ABRIR YOUTUBE EN EL NAVEGADOR
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = tutorialUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        isSpanish
                            ? $"No se pudo abrir el tutorial: {ex.Message}"
                            : $"Could not open tutorial: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }

            // ✅ SIEMPRE ABRIR LA VENTANA OBJETIVO (después del tutorial o si dijo "No")
            OpenWindow(targetWindow);
        }

        private void ManualBtn_Click(object sender, MouseButtonEventArgs e)
        {
            OpenWindow(new ManualInstallerWindow());
        }

        private void OpenWindow(Window targetWindow)
        {
            // ✅ CERRAR SOLO LAS VENTANAS SECUNDARIAS (NO MainWindow)
            var windowsToClose = Application.Current.Windows
                .Cast<Window>()
                .Where(w => w != Application.Current.MainWindow &&
                           w.IsLoaded &&
                           w.GetType().Name != "MainWindow")
                .ToList();

            foreach (var window in windowsToClose)
            {
                try
                {
                    window.Close();
                }
                catch { }
            }

            // ✅ ABRIR LA NUEVA VENTANA
            targetWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            targetWindow.Show();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}