using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ModernDesign.MVVM.View
{
    public partial class CrackedGameInstallerWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _httpClient = new HttpClient();
        private string _installPath = "";
        private readonly string _tempFolder;

        // ✅ LISTA DE ARCHIVOS PARA INSTALACIÓN CRACKEADA (SAME AS "OTHER VERSIONS")
        private readonly List<InstallFile> _installFiles = new List<InstallFile>
        {
            new InstallFile("Base Files", "https://www.mediafire.com/file_premium/hnillv3iy986uxn/Other_Files.zip/file"),
            new InstallFile("Delta Package", "https://www.mediafire.com/file_premium/m44n1u6c1d0s7un/Delta.zip/file"),
            new InstallFile("Data Package", "https://www.mediafire.com/file_premium/617ntc9sfc5e6py/Data.zip/file"),
            new InstallFile("Latest Update", "https://zeroauno.blob.core.windows.net/leuan/TheSims4/Offline/Updater/LeuanVersion/LatestLeuanVersion.zip")
        };

        public CrackedGameInstallerWindow()
        {
            InitializeComponent();
            _tempFolder = Path.Combine(Path.GetTempPath(), "LeuansSims4Toolkit_Cracked");

            if (!Directory.Exists(_tempFolder))
            {
                Directory.CreateDirectory(_tempFolder);
                try
                {
                    var di = new DirectoryInfo(_tempFolder);
                    di.Attributes |= FileAttributes.Hidden;
                }
                catch { }
            }

            ApplyLanguage();
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
            Loaded += CrackedGameInstallerWindow_Loaded;
        }

        private void ApplyLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                HeaderText.Text = "🔓 Instalando Leuan's Version...";
                SubHeaderText.Text = "Descargando la versión crackeada completa del juego (sin DLCs)";
                PathLabelText.Text = "Ubicación de instalación";
                BrowseBtn.Content = "Buscar";
                CancelBtn.Content = "❌ Cancelar";
                StartBtn.Content = "🔓 Iniciar Instalación";
                SpeedLabel.Text = "Velocidad:";
                EtaLabel.Text = "ETA:";
                StatusText.Text = "  (Seleccionar carpeta...)";
                PathTextBlock.Text = "Haz clic en 'Buscar' para seleccionar...";
            }
            else
            {
                HeaderText.Text = "🔓 Installing Leuan's Version...";
                SubHeaderText.Text = "Downloading the full cracked game (without DLCs)";
                PathLabelText.Text = "Installation location";
                BrowseBtn.Content = "Browse";
                CancelBtn.Content = "❌ Cancel";
                StartBtn.Content = "🔓 Start Installation";
                SpeedLabel.Text = "Speed:";
                EtaLabel.Text = "ETA:";
                StatusText.Text = "  (Select folder...)";
                PathTextBlock.Text = "Click 'Browse' to select...";
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

        private void CrackedGameInstallerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool isSpanish = IsSpanishLanguage();
            AddLog(isSpanish
                ? "🔓 Bienvenido al instalador de Leuan's Version (Crackeada)"
                : "🔓 Welcome to Leuan's Version installer (Cracked)");
            AddLog(isSpanish
                ? "📦 Esta versión NO incluye DLCs. Podrás descargarlos después con el toolkit."
                : "📦 This version does NOT include DLCs. You can download them later with the toolkit.");
            AddLog(isSpanish
                ? "📁 Por favor, selecciona dónde deseas instalar el juego."
                : "📁 Please select where you want to install the game.");
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isSpanish = IsSpanishLanguage();

            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = isSpanish
                    ? "Selecciona la carpeta donde deseas instalar The Sims 4"
                    : "Select the folder where you want to install The Sims 4",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetInstallPath(dialog.SelectedPath);
            }
        }

        private void SetInstallPath(string path)
        {
            bool isSpanish = IsSpanishLanguage();
            _installPath = path;
            PathTextBlock.Text = path;
            PathTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F8FAFC"));

            StatusText.Text = isSpanish ? "  (✓ Seleccionado)" : "  (✓ Selected)";
            StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#22C55E"));

            StartBtn.IsEnabled = true;

            AddLog(isSpanish
                ? $"✅ Carpeta de instalación seleccionada: {path}"
                : $"✅ Installation folder selected: {path}");
        }

        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_installPath))
                return;

            StartBtn.IsEnabled = false;
            BrowseBtn.IsEnabled = false;
            ProgressPanel.Visibility = Visibility.Visible;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await StartCrackedInstallationAsync();
            }
            catch (OperationCanceledException)
            {
                bool isSpanish = IsSpanishLanguage();
                AddLog(isSpanish ? "❌ Instalación cancelada por el usuario." : "❌ Installation cancelled by user.");
            }
            catch (Exception ex)
            {
                bool isSpanish = IsSpanishLanguage();
                AddLog(isSpanish ? $"❌ Error: {ex.Message}" : $"❌ Error: {ex.Message}");
                MessageBox.Show(
                    isSpanish
                        ? $"Error durante la instalación:\n\n{ex.Message}"
                        : $"Error during installation:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                StartBtn.IsEnabled = true;
                BrowseBtn.IsEnabled = true;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async Task StartCrackedInstallationAsync()
        {
            bool isSpanish = IsSpanishLanguage();
            int totalFiles = _installFiles.Count;

            AddLog(isSpanish
                ? $"\n🔓 Iniciando instalación de {totalFiles} paquetes..."
                : $"\n🔓 Starting installation of {totalFiles} packages...");

            for (int i = 0; i < totalFiles; i++)
            {
                var file = _installFiles[i];
                int currentIndex = i + 1;

                AddLog($"\n[{currentIndex}/{totalFiles}] {file.Name}");
                AddLog($"URL: {file.Url}");

                string tempZipPath = Path.Combine(_tempFolder, $"install_part{currentIndex}.zip");

                // Descargar
                AddLog(isSpanish ? "📥 Descargando..." : "📥 Downloading...");
                await DownloadWithProgressAsync(file.Url, tempZipPath, file.Name, currentIndex, totalFiles);

                // Extraer
                AddLog(isSpanish ? "📦 Extrayendo..." : "📦 Extracting...");
                ProgressText.Text = isSpanish
                    ? $"Extrayendo {file.Name}... ({currentIndex}/{totalFiles})"
                    : $"Extracting {file.Name}... ({currentIndex}/{totalFiles})";

                await Task.Run(() => ExtractZipWithOverwrite(tempZipPath, _installPath));

                // Eliminar ZIP
                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                    AddLog(isSpanish ? "🗑️ Archivo temporal eliminado." : "🗑️ Temporary file deleted.");
                }

                AddLog(isSpanish
                    ? $"✅ {file.Name} instalado exitosamente."
                    : $"✅ {file.Name} installed successfully.");
            }

            AddLog(isSpanish
                ? "\n✅ ¡Instalación completada exitosamente!"
                : "\n✅ Installation completed successfully!");

            ShowInstallationCompletePopup();
        }

        private void ShowInstallationCompletePopup()
        {
            bool isSpanish = IsSpanishLanguage();

            string message = isSpanish
                ? "✅ Leuan's Version ha sido instalada correctamente.\n\n" +
                  "📦 IMPORTANTE: Esta versión NO incluye DLCs.\n\n" +
                  "Puedes descargar todos los DLCs gratis usando este mismo toolkit.\n\n" +
                  "¡Disfruta tu juego!"
                : "✅ Leuan's Version has been installed successfully.\n\n" +
                  "📦 IMPORTANT: This version does NOT include DLCs.\n\n" +
                  "You can download all DLCs for free using this same toolkit.\n\n" +
                  "Enjoy your game!";

            string title = isSpanish ? "🎉 Instalación Completada" : "🎉 Installation Completed";

            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private async Task DownloadWithProgressAsync(string url, string destinationPath, string fileName, int currentIndex, int totalCount)
        {
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 0;
                var buffer = new byte[81920];
                long totalRead = 0;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var sw = Stopwatch.StartNew();
                    long lastBytesRead = 0;

                    int read;
                    while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, read, _cancellationTokenSource.Token);
                        totalRead += read;

                        if (sw.ElapsedMilliseconds >= 500)
                        {
                            UpdateProgress(totalRead, totalBytes, totalRead - lastBytesRead, sw.Elapsed.TotalSeconds, fileName, currentIndex, totalCount);
                            lastBytesRead = totalRead;
                            sw.Restart();
                        }
                    }

                    UpdateProgress(totalBytes, totalBytes, 0, 0, fileName, currentIndex, totalCount);
                }
            }
        }

        private void UpdateProgress(long bytesRead, long totalBytes, long bytesSinceLast, double secondsElapsed, string fileName, int currentIndex, int totalCount)
        {
            Dispatcher.Invoke(() =>
            {
                bool isSpanish = IsSpanishLanguage();

                ProgressText.Text = isSpanish
                    ? $"Descargando {fileName}... ({currentIndex}/{totalCount})"
                    : $"Downloading {fileName}... ({currentIndex}/{totalCount})";

                if (totalBytes > 0)
                {
                    double percent = (bytesRead * 100.0) / totalBytes;
                    ProgressPercent.Text = $"{percent:F0}%";

                    double totalWidth = ProgressPanel.ActualWidth > 0 ? ProgressPanel.ActualWidth : 400;
                    ProgressBar.Width = (percent / 100.0) * totalWidth;

                    if (secondsElapsed > 0 && bytesSinceLast > 0)
                    {
                        double speedMBps = (bytesSinceLast / secondsElapsed) / (1024 * 1024);
                        SpeedText.Text = $"{speedMBps:F2} MB/s";

                        long remainingBytes = totalBytes - bytesRead;
                        if (speedMBps > 0)
                        {
                            double remainingSeconds = remainingBytes / (speedMBps * 1024 * 1024);
                            var eta = TimeSpan.FromSeconds(remainingSeconds);
                            EtaText.Text = $"{eta:mm\\:ss}";
                        }
                    }
                }
            });
        }

        private void ExtractZipWithOverwrite(string zipPath, string destinationPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    string destinationFilePath = Path.Combine(destinationPath, entry.FullName);
                    string directoryPath = Path.GetDirectoryName(destinationFilePath);

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    entry.ExtractToFile(destinationFilePath, overwrite: true);
                }
            }
        }

        private void AddLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                LogScroller.ScrollToEnd();
            });
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            this.Close();
        }
    }

    // ✅ CLASE AUXILIAR PARA ARCHIVOS DE INSTALACIÓN
    public class InstallFile
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public InstallFile(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}