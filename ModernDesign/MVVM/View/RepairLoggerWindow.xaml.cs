using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Web.Script.Serialization;

namespace ModernDesign.MVVM.View
{
    public partial class RepairLoggerWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _httpClient = new HttpClient();
        private string _simsPath = "";
        private Dictionary<string, string> _officialHashes = new Dictionary<string, string>();
        private HashSet<string> _excludedFolders = new HashSet<string>();
        private bool _scanConfigured = false;

        private const string OFFICIAL_DATABASE_URL = "https://zeroauno.blob.core.windows.net/leuan/TheSims4/Utility/Repair/Sims4_Integrity_20251218_185419/leuan_steam_database.json";
        private const string REPAIR_BASE_URL = "https://zeroauno.blob.core.windows.net/leuan/TheSims4/Utility/Repair/";

        private int _totalFiles = 0;
        private int _scannedFiles = 0;
        private int _correctFiles = 0;
        private int _corruptFiles = 0;

        public RepairLoggerWindow()
        {
            InitializeComponent();
            ApplyLanguage();
            Loaded += RepairLoggerWindow_Loaded;
        }

        private void MainBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                try
                {
                    this.DragMove();
                }
                catch { }
            }
        }

        private void ApplyLanguage()
        {
            bool isSpanish = IsSpanishLanguage();

            if (isSpanish)
            {
                HeaderText.Text = "🔍 Verificador de Integridad del Juego";
                SubHeaderText.Text = "Compara archivos con la base de datos oficial de Steam";
                PathLabelText.Text = "Ubicación de The Sims 4";
                BrowseBtn.Content = "Buscar";
                CancelBtn.Content = "❌ Cancelar";
                StartBtn.Content = "🔍 Iniciar Escaneo";
                SpeedLabel.Text = "Velocidad:";
                EtaLabel.Text = "ETA:";
                ConfigureBtn.Content = "⚙️ Configurar Escaneo (Seleccionar DLCs)";
            }
            else
            {
                HeaderText.Text = "🔍 Game Integrity Checker";
                SubHeaderText.Text = "Compare files with official Steam database";
                PathLabelText.Text = "The Sims 4 install location";
                BrowseBtn.Content = "Browse";
                CancelBtn.Content = "❌ Cancel";
                StartBtn.Content = "🔍 Start Scan";
                SpeedLabel.Text = "Speed:";
                EtaLabel.Text = "ETA:";
                ConfigureBtn.Content = "⚙️ Configure Scan (Select DLCs)";
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

        private async void RepairLoggerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool isSpanish = IsSpanishLanguage();
            StatusText.Text = isSpanish ? "  (Buscando automáticamente...)" : "  (Searching automatically...)";

            await AutoDetectSimsPath();
        }

        private async Task AutoDetectSimsPath()
        {
            bool isSpanish = IsSpanishLanguage();

            await Task.Run(() =>
            {
                var commonPaths = new[]
                {
                    @"C:\Program Files\EA Games\The Sims 4",
                    @"C:\Program Files (x86)\EA Games\The Sims 4",
                    @"C:\Program Files\Origin Games\The Sims 4",
                    @"C:\Program Files (x86)\Origin Games\The Sims 4",
                    @"C:\Program Files (x86)\Steam\steamapps\common\The Sims 4",
                    @"D:\Games\The Sims 4",
                    @"D:\Origin Games\The Sims 4",
                    @"D:\Steam\steamapps\common\The Sims 4",
                    @"D:\The Sims 4",
                    @"E:\Games\The Sims 4",
                };

                foreach (var path in commonPaths)
                {
                    var exePath = Path.Combine(path, "Game", "Bin", "TS4_x64.exe");
                    if (File.Exists(exePath))
                    {
                        var rootPath = Directory.GetParent(Directory.GetParent(exePath).FullName).FullName;
                        rootPath = Directory.GetParent(rootPath).FullName;

                        Dispatcher.Invoke(() => SetSimsPath(rootPath, true));
                        return;
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    StatusText.Text = isSpanish
                        ? "  (No encontrado - seleccionar manualmente)"
                        : "  (Not found - select manually)";
                    StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EF4444"));

                    AddLog(isSpanish
                        ? "⚠️ No se pudo detectar automáticamente la carpeta de The Sims 4."
                        : "⚠️ Could not auto-detect The Sims 4 folder.");
                    AddLog(isSpanish
                        ? "Por favor, selecciónala manualmente usando el botón 'Buscar'."
                        : "Please select it manually using the 'Browse' button.");
                });
            });
        }

        private void SetSimsPath(string path, bool autoDetected = false)
        {
            bool isSpanish = IsSpanishLanguage();
            _simsPath = path;
            PathTextBlock.Text = path;
            PathTextBlock.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F8FAFC"));

            StatusText.Text = autoDetected
                ? (isSpanish ? "  (✓ Auto-detectado)" : "  (✓ Auto-detected)")
                : (isSpanish ? "  (✓ Seleccionado)" : "  (✓ Selected)");
            StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#22C55E"));

            // Mostrar botón de configuración
            ConfigureBtn.Visibility = Visibility.Visible;

            // Deshabilitar StartBtn hasta que se configure
            StartBtn.IsEnabled = false;

            AddLog(isSpanish
                ? $"✅ Carpeta de The Sims 4 detectada: {path}"
                : $"✅ The Sims 4 folder detected: {path}");

            AddLog(isSpanish
                ? "⚠️ Debes configurar el escaneo antes de iniciar."
                : "⚠️ You must configure the scan before starting.");
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isSpanish = IsSpanishLanguage();

            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = isSpanish
                    ? "Selecciona la carpeta de instalación de The Sims 4"
                    : "Select The Sims 4 install folder",
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var exePath = Path.Combine(dialog.SelectedPath, "Game", "Bin", "TS4_x64.exe");
                if (File.Exists(exePath) || Directory.Exists(Path.Combine(dialog.SelectedPath, "Data")))
                {
                    SetSimsPath(dialog.SelectedPath);
                }
                else
                {
                    MessageBox.Show(
                        isSpanish
                            ? "La carpeta seleccionada no parece ser una instalación válida de The Sims 4.\n\n" +
                              "Por favor selecciona la carpeta que contiene las subcarpetas 'Game' y 'Data'."
                            : "The selected folder does not look like a valid The Sims 4 installation.\n\n" +
                              "Please select the folder that contains the 'Game' and 'Data' subfolders.",
                        isSpanish ? "Ruta inválida" : "Invalid path",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
        }

        private void ConfigureBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectorWindow = new DLCSelectorWindow
            {
                Owner = this
            };

            if (selectorWindow.ShowDialog() == true)
            {
                _excludedFolders = selectorWindow.ExcludedFolders;
                _scanConfigured = true;
                StartBtn.IsEnabled = true;

                bool isSpanish = IsSpanishLanguage();
                int excludedCount = _excludedFolders.Count;

                AddLog(isSpanish
                    ? $"✅ Configuración guardada. {excludedCount} carpeta(s) excluida(s) del escaneo."
                    : $"✅ Configuration saved. {excludedCount} folder(s) excluded from scan.");

                // Actualizar texto del botón
                ConfigureBtn.Content = isSpanish
                    ? $"⚙️ Reconfigurar Escaneo ({excludedCount} excluidas)"
                    : $"⚙️ Reconfigure Scan ({excludedCount} excluded)";
            }
        }

        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_simsPath))
                return;

            StartBtn.IsEnabled = false;
            BrowseBtn.IsEnabled = false;
            ConfigureBtn.IsEnabled = false;
            ProgressPanel.Visibility = Visibility.Visible;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await StartIntegrityCheckAsync();
            }
            catch (OperationCanceledException)
            {
                bool isSpanish = IsSpanishLanguage();
                AddLog(isSpanish ? "❌ Escaneo cancelado por el usuario." : "❌ Scan cancelled by user.");
            }
            catch (Exception ex)
            {
                bool isSpanish = IsSpanishLanguage();
                AddLog(isSpanish ? $"❌ Error: {ex.Message}" : $"❌ Error: {ex.Message}");
                MessageBox.Show(
                    isSpanish
                        ? $"Error durante el escaneo:\n\n{ex.Message}"
                        : $"Error during scan:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                StartBtn.IsEnabled = _scanConfigured;
                BrowseBtn.IsEnabled = true;
                ConfigureBtn.IsEnabled = true;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async Task StartIntegrityCheckAsync()
        {
            bool isSpanish = IsSpanishLanguage();

            if (!_scanConfigured)
            {
                MessageBox.Show(
                    isSpanish
                        ? "Debes configurar el escaneo primero usando el botón de configuración."
                        : "You must configure the scan first using the configuration button.",
                    isSpanish ? "Configuración Requerida" : "Configuration Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            AddLog(isSpanish
                ? "\n🔍 Iniciando verificación de integridad..."
                : "\n🔍 Starting integrity check...");

            // Step 1: Download official database
            AddLog(isSpanish
                ? "📥 Descargando base de datos oficial de Steam..."
                : "📥 Downloading official Steam database...");

            await DownloadOfficialDatabaseAsync();

            AddLog(isSpanish
                ? $"✅ Base de datos descargada. Total de archivos oficiales: {_officialHashes.Count}"
                : $"✅ Database downloaded. Total official files: {_officialHashes.Count}");

            // Step 2: Prepare verification
            AddLog(isSpanish
                ? "\n🔎 Preparando verificación contra base de datos oficial..."
                : "\n🔎 Preparing verification against official database...");

            var corruptedFiles = new List<string>();
            var sw = Stopwatch.StartNew();

            // Step 3: Verify each file from official database
            var missingFiles = new List<string>();

            // Filtrar archivos de la base de datos según carpetas seleccionadas
            var filteredOfficialHashes = _officialHashes
                .Where(kvp =>
                {
                    var parts = kvp.Key.Split('/');
                    if (parts.Length == 0) return false;

                    var rootFolder = parts[0];

                    // Si está en la lista de excluidos, no verificar
                    return !_excludedFolders.Contains(rootFolder);
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            _totalFiles = filteredOfficialHashes.Count;
            _scannedFiles = 0;
            _correctFiles = 0;
            _corruptFiles = 0;

            AddLog(isSpanish
                ? $"📊 Total de archivos a verificar (según base de datos): {_totalFiles}"
                : $"📊 Total files to verify (from database): {_totalFiles}");

            foreach (var officialEntry in filteredOfficialHashes)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                var normalizedPath = officialEntry.Key;
                var officialHash = officialEntry.Value;
                var localPath = Path.Combine(_simsPath, normalizedPath.Replace("/", "\\"));

                UpdateCurrentFile(normalizedPath);
                _scannedFiles++;

                // Verificar si el archivo existe localmente
                if (!File.Exists(localPath))
                {
                    _corruptFiles++;
                    missingFiles.Add(normalizedPath);
                    corruptedFiles.Add(normalizedPath);
                    AddLog(isSpanish
                        ? $"❌ {normalizedPath} | FALTA ARCHIVO"
                        : $"❌ {normalizedPath} | MISSING FILE");
                }
                else
                {
                    // El archivo existe, verificar hash
                    var localHash = await Task.Run(() => CalculateSHA256(localPath));

                    if (localHash.Equals(officialHash, StringComparison.OrdinalIgnoreCase))
                    {
                        _correctFiles++;
                        AddLog(isSpanish
                            ? $"✅ {normalizedPath}"
                            : $"✅ {normalizedPath}");
                    }
                    else
                    {
                        _corruptFiles++;
                        corruptedFiles.Add(normalizedPath);
                        AddLog(isSpanish
                            ? $"❌ {normalizedPath} | HASH INCORRECTO"
                            : $"❌ {normalizedPath} | INCORRECT HASH");
                    }
                }

                UpdateProgress(_scannedFiles, _totalFiles);
                UpdateStats();
            }

            if (missingFiles.Count > 0)
            {
                AddLog(isSpanish
                    ? $"\n⚠️ Se detectaron {missingFiles.Count} archivo(s) faltante(s)"
                    : $"\n⚠️ Detected {missingFiles.Count} missing file(s)");
            }

            sw.Stop();

            // Step 4: Summary
            AddLog(isSpanish
                ? $"\n📊 Escaneo completado en {sw.Elapsed.TotalSeconds:F2} segundos"
                : $"\n📊 Scan completed in {sw.Elapsed.TotalSeconds:F2} seconds");
            AddLog(isSpanish
                ? $"   Total: {_totalFiles} | Correctos: {_correctFiles} | Corruptos: {_corruptFiles}"
                : $"   Total: {_totalFiles} | Valid: {_correctFiles} | Corrupt: {_corruptFiles}");

            // Step 5: Repair corrupted files
            if (corruptedFiles.Count > 0)
            {
                var result = MessageBox.Show(
                    isSpanish
                        ? $"Se encontraron {corruptedFiles.Count} archivo(s) corrupto(s).\n\n¿Deseas repararlos automáticamente descargándolos desde el servidor oficial?"
                        : $"Found {corruptedFiles.Count} corrupt file(s).\n\nDo you want to automatically repair them by downloading from the official server?",
                    isSpanish ? "Archivos Corruptos Detectados" : "Corrupt Files Detected",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    await RepairCorruptedFilesAsync(corruptedFiles);
                }
            }
            else
            {
                MessageBox.Show(
                    isSpanish
                        ? "✅ ¡Todos los archivos están correctos!\n\nTu instalación de The Sims 4 está actualizada y sin errores."
                        : "✅ All files are valid!\n\nYour The Sims 4 installation is up-to-date and error-free.",
                    isSpanish ? "Verificación Completada" : "Verification Completed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private async Task DownloadOfficialDatabaseAsync()
        {
            var jsonContent = await _httpClient.GetStringAsync(OFFICIAL_DATABASE_URL);

            var serializer = new JavaScriptSerializer();
            var jsonObj = serializer.Deserialize<Dictionary<string, object>>(jsonContent);

            _officialHashes.Clear();

            if (jsonObj != null && jsonObj.ContainsKey("files"))
            {
                var filesObj = jsonObj["files"] as Dictionary<string, object>;
                if (filesObj != null)
                {
                    foreach (var kvp in filesObj)
                    {
                        // Normalizar la ruta: convertir backslashes a forward slashes
                        var normalizedKey = kvp.Key.Replace("\\", "/");
                        _officialHashes[normalizedKey] = kvp.Value.ToString().ToLowerInvariant();
                    }
                }
            }
        }

        private string CalculateSHA256(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private async Task RepairCorruptedFilesAsync(List<string> corruptedFiles)
        {
            bool isSpanish = IsSpanishLanguage();

            AddLog(isSpanish
                ? $"\n🔧 Iniciando reparación de {corruptedFiles.Count} archivo(s)..."
                : $"\n🔧 Starting repair of {corruptedFiles.Count} file(s)...");

            int repairedCount = 0;
            int failedCount = 0;

            for (int i = 0; i < corruptedFiles.Count; i++)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                var normalizedPath = corruptedFiles[i]; // Ya viene normalizado con /
                var downloadUrl = REPAIR_BASE_URL + normalizedPath;
                var localPath = Path.Combine(_simsPath, normalizedPath.Replace("/", "\\"));

                AddLog(isSpanish
                    ? $"\n[{i + 1}/{corruptedFiles.Count}] Reparando: {normalizedPath}"
                    : $"\n[{i + 1}/{corruptedFiles.Count}] Repairing: {normalizedPath}");

                try
                {
                    // Asegurar que el directorio existe
                    var directory = Path.GetDirectoryName(localPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Descargar archivo
                    await DownloadFileWithProgressAsync(downloadUrl, localPath, normalizedPath, i + 1, corruptedFiles.Count);

                    // Verificar hash después de la descarga
                    var newHash = await Task.Run(() => CalculateSHA256(localPath));
                    if (_officialHashes.TryGetValue(normalizedPath, out var officialHash))
                    {
                        if (newHash.Equals(officialHash, StringComparison.OrdinalIgnoreCase))
                        {
                            repairedCount++;
                            AddLog(isSpanish
                                ? $"✅ Reparado exitosamente: {normalizedPath}"
                                : $"✅ Successfully repaired: {normalizedPath}");
                        }
                        else
                        {
                            failedCount++;
                            AddLog(isSpanish
                                ? $"❌ Error: Hash no coincide después de descargar: {normalizedPath}"
                                : $"❌ Error: Hash mismatch after download: {normalizedPath}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    AddLog(isSpanish
                        ? $"❌ Error al reparar {normalizedPath}: {ex.Message}"
                        : $"❌ Error repairing {normalizedPath}: {ex.Message}");
                }
            }

            AddLog(isSpanish
                ? $"\n📊 Reparación completada: {repairedCount} exitosos, {failedCount} fallidos"
                : $"\n📊 Repair completed: {repairedCount} successful, {failedCount} failed");

            MessageBox.Show(
                isSpanish
                    ? $"Reparación completada:\n\n✅ Reparados: {repairedCount}\n❌ Fallidos: {failedCount}"
                    : $"Repair completed:\n\n✅ Repaired: {repairedCount}\n❌ Failed: {failedCount}",
                isSpanish ? "Reparación Completada" : "Repair Completed",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private async Task DownloadFileWithProgressAsync(string url, string destinationPath, string fileName, int currentIndex, int totalCount)
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
                            UpdateDownloadProgress(totalRead, totalBytes, totalRead - lastBytesRead, sw.Elapsed.TotalSeconds, fileName, currentIndex, totalCount);
                            lastBytesRead = totalRead;
                            sw.Restart();
                        }
                    }

                    UpdateDownloadProgress(totalBytes, totalBytes, 0, 0, fileName, currentIndex, totalCount);
                }
            }
        }

        private void UpdateCurrentFile(string fileName)
        {
            Dispatcher.Invoke(() =>
            {
                CurrentFileText.Text = $"({Path.GetFileName(fileName)})";
            });
        }

        private void UpdateProgress(int current, int total)
        {
            Dispatcher.Invoke(() =>
            {
                if (total > 0)
                {
                    double percent = (current * 100.0) / total;
                    ProgressPercent.Text = $"{percent:F1}%";

                    double totalWidth = ProgressPanel.ActualWidth > 0 ? ProgressPanel.ActualWidth : 700;
                    ProgressBar.Width = (percent / 100.0) * totalWidth;
                }
            });
        }

        private void UpdateStats()
        {
            Dispatcher.Invoke(() =>
            {
                bool isSpanish = IsSpanishLanguage();
                StatsText.Text = isSpanish
                    ? $"Archivos: {_scannedFiles}/{_totalFiles} | Correctos: {_correctFiles} | Corruptos: {_corruptFiles}"
                    : $"Files: {_scannedFiles}/{_totalFiles} | Valid: {_correctFiles} | Corrupt: {_corruptFiles}";
            });
        }

        private void UpdateDownloadProgress(long bytesRead, long totalBytes, long bytesSinceLast, double secondsElapsed, string fileName, int currentIndex, int totalCount)
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

                    double totalWidth = ProgressPanel.ActualWidth > 0 ? ProgressPanel.ActualWidth : 700;
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
}