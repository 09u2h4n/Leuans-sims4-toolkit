using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ModernDesign.MVVM.View
{
    public partial class leuFastSomeDLCsWindow : Window
    {
        private const string TorrentUrl = "https://download1076.mediafire.com/75vh48qlmxygb2VVBl5chgURqabJmAlE6Mz5uPF3cmOYku1bTcu2-tKMmp1QlKduEjeSk-20svX2TDhdIfHhTgarRbzWaMpG3FRS8SgpCDUfAH0z3Zwh99sB2_JG4tzg9J_7euONZAG-TuHbdK05Gv5kQe_e2d6pjRF_OMsP5ccX/sd9kovbcpp9zsp8/The+Sims+4+-+All+DLCs.torrent";

        public leuFastSomeDLCsWindow()
        {
            InitializeComponent();
            Loaded += leuFastSomeDLCsWindow_Loaded;
        }

        private void leuFastSomeDLCsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguage();
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
                HeaderTitle.Text = "🎲  Tutorial: Instalar Algunos DLCs";
                Title.Text = "BitTorrent Requerido";
                Description.Text = "Con la versión actual, solo puedes instalar algunos DLCs usando BitTorrent.\n\n" +
                                   "Haz clic en el botón de abajo para descargar el archivo BitTorrent.";
                DownloadBtn.Content = "🌐 Descargar Archivo BitTorrent";
            }
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = TorrentUrl,
                    UseShellExecute = true
                });

                bool isSpanish = IsSpanishLanguage();
                MessageBox.Show(
                    isSpanish
                        ? "El archivo BitTorrent se está descargando.\n\nUsa un cliente BitTorrent para descargar los DLCs."
                        : "BitTorrent file is downloading.\n\nUse a BitTorrent client to download the DLCs.",
                    isSpanish ? "Descarga Iniciada" : "Download Started",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var welcomeWindow = new leuFastWelcomeWindow();

            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            fadeOut.Completed += (s, args) =>
            {
                this.Close();
                welcomeWindow.Opacity = 0;
                welcomeWindow.Show();

                var fadeIn = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };
                welcomeWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
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