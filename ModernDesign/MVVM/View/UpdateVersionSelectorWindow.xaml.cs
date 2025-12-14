using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernDesign.MVVM.View
{
    public partial class UpdateVersionSelectorWindow : Window
    {
        public UpdateVersionSelectorWindow()
        {
            InitializeComponent();
            ApplyLanguage();
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
        }

        private void ApplyLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                HeaderText.Text = "🔄 Selecciona tu versión";
                SubHeaderText.Text = "Elige la versión de tu juego";
                LeuanVersionBtn.Content = "Leuan's Version";
                OtherVersionsBtn.Content = "Other Versions";

                UpdateButtonSubtitle(LeuanVersionBtn, "Versión oficial de Leuan");
                UpdateButtonSubtitle(OtherVersionsBtn, "Como Anadius, FitGirl, etc.");
            }
            else
            {
                HeaderText.Text = "🔄 Select your version";
                SubHeaderText.Text = "Choose your game version";
                LeuanVersionBtn.Content = "Leuan's Version";
                OtherVersionsBtn.Content = "Other Versions";

                UpdateButtonSubtitle(LeuanVersionBtn, "Official Leuan version");
                UpdateButtonSubtitle(OtherVersionsBtn, "Such as Anadius, FitGirl, etc.");
            }
        }

        private void UpdateButtonSubtitle(Button button, string text)
        {
            button.Loaded += (s, e) =>
            {
                var template = button.Template;
                if (template != null)
                {
                    var subtitle = template.FindName("subtitle", button) as TextBlock;
                    if (subtitle != null)
                        subtitle.Text = text;
                }
            };
        }

        private static bool IsSpanishLanguage()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string languagePath = System.IO.Path.Combine(appData, "Leuan's - Sims 4 ToolKit", "language.ini");

                if (!System.IO.File.Exists(languagePath))
                    return false;

                var lines = System.IO.File.ReadAllLines(languagePath);
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

        private void LeuanVersionBtn_Click(object sender, RoutedEventArgs e)
        {
            // Abrir logger para Leuan's Version
            var loggerWindow = new UpdateLoggerWindow("leuan");
            loggerWindow.Show();
            this.Close();
        }

        private void OtherVersionsBtn_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar OfflineWarningWindow3 y luego logger
            var warningWindow = new OfflineWarningWindow3
            {
                Owner = this
            };

            bool? result = warningWindow.ShowDialog();

            if (result == true && warningWindow.UserConfirmed)
            {
                var loggerWindow = new UpdateLoggerWindow("other");
                loggerWindow.Show();
                this.Close();
            }
        }
    }
}