using System;
using System.IO;
using System.Windows;

namespace ModernDesign.MVVM.View
{
    public partial class InstallMethodSelectorWindow : Window
    {
        public InstallMethodSelectorWindow()
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
                HeaderText.Text = "🎮 Elige tu Método de Instalación";
                SubHeaderText.Text = "Selecciona cómo deseas instalar The Sims 4";

                LegitTitle.Text = "Copia Legítima";
                LegitDescription.Text = "Tengo o quiero comprar el juego desde Steam o EA App";
                LegitBtn.Content = "Elegir Legítima";

                CrackedTitle.Text = "Versión Crackeada";
                CrackedDescription.Text = "Quiero descargar el juego completo crackeado (Leuan's Version)";
                CrackedBtn.Content = "Elegir Crackeada";

                CloseBtn.Content = "❌ Cerrar";
            }
            else
            {
                HeaderText.Text = "🎮 Choose Your Installation Method";
                SubHeaderText.Text = "Select how you want to install The Sims 4";

                LegitTitle.Text = "Legit Copy";
                LegitDescription.Text = "I want to have a legit copy of the game from Steam or EA App";
                LegitBtn.Content = "Choose Legit";

                CrackedTitle.Text = "Cracked Version";
                CrackedDescription.Text = "Doesn't need Steam or EA, not even Internet to play it";
                CrackedBtn.Content = "Choose Cracked";

                CloseBtn.Content = "❌ Close";
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

        private void LegitBtn_Click(object sender, RoutedEventArgs e)
        {
            var legitTutorial = new LegitInstallTutorialWindow();
            legitTutorial.ShowDialog();
        }

        private void CrackedBtn_Click(object sender, RoutedEventArgs e)
        {
            var crackedInstaller = new PlugnPlaySelectorWindow();
            crackedInstaller.ShowDialog();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}