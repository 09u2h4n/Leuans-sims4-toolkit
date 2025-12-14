using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ModernDesign.MVVM.View
{
    public partial class TutorialMainWindow : Window
    {
        public TutorialMainWindow()
        {
            InitializeComponent();
            LoadLanguage();

            this.MouseLeftButtonDown += Window_MouseLeftButtonDown;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Iniciar animaciones en secuencia
            PlayAnimations();
        }

        private async void PlayAnimations()
        {
            // 1. Fade in del fondo
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");
            fadeIn.Begin();

            await Task.Delay(200);

            // 2. Animar header (slide from top)
            AnimateElement(HeaderPanel, "SlideInFromLeft");

            await Task.Delay(300);

            // 3. Animar Step 1 con glow effect
            AnimateStepWithGlow(Step1Border, "SlideInFromLeft", "#3B82F6");

            await Task.Delay(400);

            // 4. Animar Step 2 con glow effect
            AnimateStepWithGlow(Step2Border, "SlideInFromRight", "#22C55E");

            await Task.Delay(400);

            // 5. Animar Step 3 con glow effect
            AnimateStepWithGlow(Step3Border, "SlideInFromLeft", "#F59E0B");

            await Task.Delay(400);

            // 6. Animar footer
            AnimateElement(FooterPanel, "SlideInFromRight");
        }

        private void AnimateElement(FrameworkElement element, string storyboardKey)
        {
            var storyboard = (Storyboard)FindResource(storyboardKey);
            storyboard.Begin(element);
        }

        private async void AnimateStepWithGlow(FrameworkElement element, string storyboardKey, string glowColor)
        {
            // Animar entrada
            var storyboard = (Storyboard)FindResource(storyboardKey);
            storyboard.Begin(element);

            // Animar glow effect
            if (element.Effect is DropShadowEffect shadowEffect)
            {
                var glowAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 0.6,
                    Duration = TimeSpan.FromSeconds(0.8),
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(1)
                };

                shadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, glowAnimation);
            }
        }

        private void LoadLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                TutorialTitle.Text = "🎓 Tutorial Rápido";
                TutorialSubtitle.Text = "Aprende lo básico en 3 simples pasos";

                Step1Title.Text = "Selecciona la Ruta de tu Juego";
                Step1Description.Text =
                    "Primero, debes indicarle al ToolKit dónde está instalado The Sims 4 en tu computadora.\n\n" +
                    "• Haz clic en 'Buscar' o 'Auto-Detectar'\n" +
                    "• La ruta debería verse así: C:\\Program Files\\EA Games\\The Sims 4";

                Step2Title.Text = "Elige tus DLCs";
                Step2Description.Text =
                    "Navega por la lista de DLCs disponibles y selecciona lo que quieras instalar.\n\n" +
                    "• Usa 'Seleccionar Todo' para instalar todo\n" +
                    "• O elige packs individuales que te interesen\n" +
                    "• Los DLCs ya instalados estarán marcados con un ✓ verde";

                Step3Title.Text = "Descarga e Instala";
                Step3Description.Text =
                    "¡Haz clic en el botón Descargar y deja que el ToolKit haga su magia!\n\n" +
                    "• El ToolKit descargará e instalará todo automáticamente\n" +
                    "• Puedes ver el progreso, velocidad y tiempo estimado en tiempo real\n" +
                    "• Una vez terminado, instala el EA DLC Unlocker para activar todos los DLCs";

                GotItBtn.Content = "✓ ¡Entendido, empecemos!";
                SkipBtn.Content = "Saltar Tutorial";
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

        private void GotItBtn_Click(object sender, RoutedEventArgs e)
        {
            // Cerrar el tutorial
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void SkipBtn_Click(object sender, RoutedEventArgs e)
        {
            // Cerrar el tutorial
            this.Close();
        }
    }
}