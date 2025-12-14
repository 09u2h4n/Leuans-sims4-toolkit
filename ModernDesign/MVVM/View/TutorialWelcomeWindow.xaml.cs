using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ModernDesign.MVVM.View
{
    public partial class TutorialWelcomeWindow : Window
    {
        public TutorialWelcomeWindow()
        {
            InitializeComponent();
            LoadLanguage();

            // Permitir cerrar con ESC
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    this.Close();
                }
            };
        }

        private void LoadLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                WelcomeTitle.Text = "👋 ¡Bienvenido a Leuan's Sims 4 ToolKit!";
                WelcomeSubtitle.Text = "Comencemos con un tutorial rápido";
                QuestionText.Text = "¿Ya tienes The Sims 4 instalado en tu PC?";
                YesBtn.Content = "✓ Sí, lo tengo";
                NoBtn.Content = "✗ No, lo necesito";
                SkipText.Text = "Puedes saltar este tutorial en cualquier momento presionando ESC";
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

        private void YesBtn_Click(object sender, RoutedEventArgs e)
        {
            // Usuario ya tiene el juego - mostrar tutorial principal
            var mainTutorial = new TutorialMainWindow();
            mainTutorial.Owner = this.Owner;
            mainTutorial.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Close();
            mainTutorial.ShowDialog();
        }

        private void NoBtn_Click(object sender, RoutedEventArgs e)
        {
            // Usuario NO tiene el juego - mostrar ventana de descarga
            var downloadGameWindow = new InstallMethodSelectorWindow();
            downloadGameWindow.Owner = this.Owner;
            downloadGameWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Close();
            downloadGameWindow.ShowDialog();
        }
    }
}