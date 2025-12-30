using LeuanS4ToolKit.MVVM.View;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ModernDesign.MVVM.View
{
    public partial class PlugnPlaySelectorWindow : Window
    {
        public PlugnPlaySelectorWindow()
        {
            InitializeComponent();
            ApplyLanguage();

            // ✅ SOLO aplicar DragMove en el Grid principal, NO en elementos clickeables
            this.MouseLeftButtonDown += Window_MouseLeftButtonDown;
        }

        // ✅ NUEVO: Manejador que verifica si el clic es en un área válida para drag
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Solo hacer DragMove si NO se clickeó en el banner de AntiDescarga
            var clickedElement = e.OriginalSource as FrameworkElement;

            // Verificar si el clic fue en el banner o sus hijos
            bool isAntiDescargaBanner = false;
            var current = clickedElement;
            while (current != null)
            {
                if (current.Name == "AntiDescargaBanner")
                {
                    isAntiDescargaBanner = true;
                    break;
                }
                current = current.Parent as FrameworkElement;
            }

            // Solo hacer DragMove si NO es el banner
            if (!isAntiDescargaBanner)
            {
                try
                {
                    this.DragMove();
                }
                catch
                {
                    // Ignorar errores de DragMove
                }
            }
        }

        private void ApplyLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                HeaderText.Text = "🎮 Instalación Plug'n'Play";
                SubHeaderText.Text = "Elige tu paquete de instalación";

                AntiDescargaText.Text = "Omite el Proceso de Descarga - ¡Haz Clic Aquí!";

                FullGameTitle.Text = "Juego Base + DLCs";
                FullGameSize.Text = "📦 Requerido: ~140GB";
                FullGameDesc.Text = "(Descarga + Extracción)";
                FullGameFeatures.Text = "✅ Juego completo con TODOS los DLCs\n" +
                                       "✅ Listo para jugar instantáneamente\n" +
                                       "✅ Sin descargas adicionales";
                FullGameBtn.Content = "Descargar Paquete Completo";

                BaseGameTitle.Text = "Solo Juego Base";
                BaseGameSize.Text = "📦 Requerido: ~40GB";
                BaseGameDesc.Text = "(Descarga + Extracción)";
                BaseGameFeatures.Text = "✅ Solo juego base\n" +
                                       "✅ Tamaño de descarga menor\n" +
                                       "✅ Agrega DLCs después con el toolkit";
                BaseGameBtn.Content = "Descargar Juego Base";

                CloseBtn.Content = "❌ Cerrar";
            }
            else
            {
                HeaderText.Text = "🎮 Plug'n'Play Installation";
                SubHeaderText.Text = "Choose your installation package";

                AntiDescargaText.Text = "Skip Downloading Process - Click Here!";

                FullGameTitle.Text = "Game Base + DLCs";
                FullGameSize.Text = "📦 Required: ~140GB";
                FullGameDesc.Text = "(Download + Extraction)";
                FullGameFeatures.Text = "✅ Complete game with ALL DLCs\n" +
                                       "✅ Ready to play instantly\n" +
                                       "✅ No additional downloads";
                FullGameBtn.Content = "Download Full Package";

                BaseGameTitle.Text = "Game Base Only";
                BaseGameSize.Text = "📦 Required: ~40GB";
                BaseGameDesc.Text = "(Download + Extraction)";
                BaseGameFeatures.Text = "✅ Base game only\n" +
                                       "✅ Smaller download size\n" +
                                       "✅ Add DLCs later with toolkit";
                BaseGameBtn.Content = "Download Base Game";

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

        private void AntiDescargaPromo_Click(object sender, MouseButtonEventArgs e)
        {
            // ✅ Marcar el evento como manejado para evitar que se propague
            e.Handled = true;

            bool isSpanish = IsSpanishLanguage();

            string message = isSpanish
                ? "⚡ ¡Omite el Proceso de Descarga!\n\n" +
                  "Los supporters mensuales obtienen acceso a un Cloud Premium donde pueden omitir " +
                  "el proceso de descargar los archivos.\n\n" +
                  "Leuan pre-descarga todo por ti, así que solo debes acceder a la carpeta compartida " +
                  "de Leuan y extraer directamente a tu PC.\n\n" +
                  "💎 Beneficios:\n" +
                  "• Ahorra tiempo de descarga\n" +
                  "• Acceso instantáneo a archivos\n" +
                  "• Actualizaciones prioritarias\n\n" +
                  "¿Quieres obtener este beneficio? ¡Apoya en Ko-fi!"
                : "⚡ Skip the Downloading Process!\n\n" +
                  "Monthly supporters get access to a Premium Cloud where they can skip " +
                  "the file downloading process.\n\n" +
                  "Leuan pre-downloads everything for you, so you just need to access Leuan's " +
                  "shared folder and extract directly to your PC.\n\n" +
                  "💎 Benefits:\n" +
                  "• Save download time\n" +
                  "• Instant access to files\n" +
                  "• Priority updates\n\n" +
                  "Want to get this benefit? Support on Ko-fi!";

            string title = isSpanish ? "⚡ AntiDescarga Premium" : "⚡ AntiDescarga Premium";

            var result = MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = "https://ko-fi.com/leuan",
                        UseShellExecute = false
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open Ko-fi: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FullGameBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isSpanish = IsSpanishLanguage();

            // Abrir la web
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "https://leuan.zeroauno.com/sims4-toolkit/sims4fullgame.html",
                    UseShellExecute = false
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open website: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Mostrar popup de instrucciones
            string message = isSpanish
                ? "📥 Instrucciones de Descarga:\n\n" +
                  "1. Descarga TODAS las partes del archivo\n" +
                  "2. Extrae los archivos en tu ubicación deseada\n" +
                  "3. ¡Disfruta jugando!\n\n" +
                  "❓ ¿Necesitas ayuda?\n" +
                  "Dile al chatbot 'Discord' y consulta ayuda en nuestro servidor de Discord."
                : "📥 Download Instructions:\n\n" +
                  "1. Download ALL parts of the file\n" +
                  "2. Extract the files to your desired location\n" +
                  "3. Enjoy playing!\n\n" +
                  "❓ Need help?\n" +
                  "Tell the chatbot 'Discord' and get help on our Discord server.";

            string title = isSpanish ? "📥 Instrucciones" : "📥 Instructions";

            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BaseGameBtn_Click(object sender, RoutedEventArgs e)
        {
            var baseGameInstaller = new CrackedGameInstallerWindow();
            baseGameInstaller.ShowDialog();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}