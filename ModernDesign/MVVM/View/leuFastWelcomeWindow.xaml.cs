using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernDesign.MVVM.View
{
    public partial class leuFastWelcomeWindow : Window
    {
        public leuFastWelcomeWindow()
        {
            InitializeComponent();
            Loaded += leuFastWelcomeWindow_Loaded;
        }

        private void leuFastWelcomeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguage();
            AnimateEntrance();
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

        private void ApplyLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                WelcomeTitle.Text = "¡Bienvenido a la Instalación de DLCs!";
                WelcomeSubtitle.Text = "¿Qué te gustaría hacer?";

                InstallAllTitle.Text = "Instalar Todos los DLCs";
                InstallAllDesc.Text = "Paquete completo con todo";

                InstallSomeTitle.Text = "Instalar Algunos DLCs";
                InstallSomeDesc.Text = "Elige contenido específico";

                InstallBaseTitle.Text = "Juego Base + DLCs";
                InstallBaseDesc.Text = "Instalación completa desde cero";
            }
        }

        private void AnimateEntrance()
        {
            // Fade in window
            var windowFade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            this.BeginAnimation(Window.OpacityProperty, windowFade);

            // Animate circle - scale and pulse
            var scaleTransform = new ScaleTransform(0.5, 0.5);
            WelcomeCircle.RenderTransform = scaleTransform;
            WelcomeCircle.RenderTransformOrigin = new Point(0.5, 0.5);

            var scaleAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(600),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 }
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            // Pulse animation for circle
            var pulseAnimation = new DoubleAnimationUsingKeyFrames
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            pulseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            pulseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.05, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));
            pulseAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2))));

            var pulseTransform = new ScaleTransform(1, 1);
            WelcomeCircle.RenderTransform = new TransformGroup
            {
                Children = { scaleTransform, pulseTransform }
            };

            pulseTransform.BeginAnimation(ScaleTransform.ScaleXProperty, pulseAnimation);
            pulseTransform.BeginAnimation(ScaleTransform.ScaleYProperty, pulseAnimation);

            // Fade in title
            WelcomeTitle.Opacity = 0;
            var titleFade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                BeginTime = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            WelcomeTitle.BeginAnimation(OpacityProperty, titleFade);

            // Fade in subtitle
            WelcomeSubtitle.Opacity = 0;
            var subtitleFade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                BeginTime = TimeSpan.FromMilliseconds(400),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            WelcomeSubtitle.BeginAnimation(OpacityProperty, subtitleFade);

            // Fade in options with stagger
            OptionsGrid.Opacity = 0;
            var optionsFade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(600),
                BeginTime = TimeSpan.FromMilliseconds(600),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            OptionsGrid.BeginAnimation(OpacityProperty, optionsFade);
        }

        private void InstallAllBtn_Click(object sender, RoutedEventArgs e)
        {
            var tutorialWindow = new leuFastAllDLCsWindow
            {
                Owner = this
            };

            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            fadeOut.Completed += (s, args) =>
            {
                this.Hide();
                tutorialWindow.Opacity = 0;
                tutorialWindow.Show();

                var fadeIn = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };
                tutorialWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void InstallSomeBtn_Click(object sender, RoutedEventArgs e)
        {
            var tutorialWindow = new leuFastSomeDLCsWindow
            {
                Owner = this
            };

            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            fadeOut.Completed += (s, args) =>
            {
                this.Hide();
                tutorialWindow.Opacity = 0;
                tutorialWindow.Show();

                var fadeIn = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };
                tutorialWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void InstallBaseBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isSpanish = IsSpanishLanguage();

            MessageBox.Show(
                isSpanish
                    ? "Consulta en el Discord para instrucciones manuales.\n\nEsta pestaña será creada en el futuro con más detalle."
                    : "Check Discord for manual instructions.\n\nThis tab will be created in the future with more detail.",
                isSpanish ? "Próximamente" : "Coming Soon",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Open Discord
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://discord.gg/your-discord-link",
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            fadeOut.Completed += (s, args) => this.Close();
            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }
    }
}