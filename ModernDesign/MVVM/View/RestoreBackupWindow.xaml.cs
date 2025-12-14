using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace ModernDesign.MVVM.View
{
    public class BackupInfo
    {
        public string Timestamp { get; set; }
        public string SlotId { get; set; }
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
        public string SizeFormatted { get; set; }
        public List<string> Files { get; set; } = new List<string>();
    }

    public partial class RestoreBackupWindow : Window
    {
        private string _savesFolder;
        private string _languageCode;
        private List<BackupInfo> _backups = new List<BackupInfo>();

        public RestoreBackupWindow(string savesFolder, string languageCode)
        {
            InitializeComponent();
            _savesFolder = savesFolder;
            _languageCode = languageCode;

            InitTexts();
            LoadBackups();
        }

        private void InitTexts()
        {
            bool es = _languageCode.StartsWith("es", StringComparison.OrdinalIgnoreCase);

            Title = es ? "Restaurar Savegame" : "Restore Savegame";
            TitleText.Text = es ? "♻️ Restaurar Savegame" : "♻️ Restore Savegame";
            SubtitleText.Text = es ? "Selecciona un backup para restaurar" : "Select a backup to restore";
            RestoreSelectedButton.Content = es ? "♻️ Restaurar Seleccionado" : "♻️ Restore Selected";
            CancelButton.Content = es ? "Cancelar" : "Cancel";
        }

        private void LoadBackups()
        {
            _backups.Clear();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string backupFolder = Path.Combine(appData, "Leuan's - Sims 4 ToolKit", "savegame_backups");

            if (!Directory.Exists(backupFolder))
            {
                BackupsListView.ItemsSource = _backups;
                return;
            }

            var files = Directory.GetFiles(backupFolder, "*.save*");

            // Regex para extraer: "2025-01-23_14-30-00_Slot_00000001.save"
            var regex = new Regex(@"^(\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2})_(Slot_\d+)\.save", RegexOptions.IgnoreCase);

            var groups = files
                .Select(f => new
                {
                    Path = f,
                    Name = Path.GetFileName(f),
                    Match = regex.Match(Path.GetFileName(f))
                })
                .Where(x => x.Match.Success)
                .Select(x => new
                {
                    x.Path,
                    Timestamp = x.Match.Groups[1].Value,
                    SlotId = x.Match.Groups[2].Value
                })
                .GroupBy(x => new { x.Timestamp, x.SlotId });

            foreach (var group in groups)
            {
                var fileList = group.Select(g => g.Path).ToList();
                long totalSize = fileList.Sum(f => new FileInfo(f).Length);

                _backups.Add(new BackupInfo
                {
                    Timestamp = group.Key.Timestamp.Replace("_", " "),
                    SlotId = group.Key.SlotId,
                    FileCount = fileList.Count,
                    TotalSize = totalSize,
                    SizeFormatted = FormatBytes(totalSize),
                    Files = fileList
                });
            }

            // Ordenar por fecha descendente
            _backups = _backups.OrderByDescending(b => b.Timestamp).ToList();

            BackupsListView.ItemsSource = _backups;
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void RestoreSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            bool es = _languageCode.StartsWith("es", StringComparison.OrdinalIgnoreCase);

            if (BackupsListView.SelectedItem == null)
            {
                MessageBox.Show(
                    es ? "Por favor selecciona un backup de la lista."
                       : "Please select a backup from the list.",
                    es ? "Sin selección" : "No selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var selected = (BackupInfo)BackupsListView.SelectedItem;

            var result = MessageBox.Show(
                es ? $"¿Estás seguro de que deseas restaurar este backup?\n\n" +
                     $"Slot: {selected.SlotId}\n" +
                     $"Fecha: {selected.Timestamp}\n" +
                     $"Archivos: {selected.FileCount}\n\n" +
                     $"⚠️ ADVERTENCIA: Esto sobrescribirá tus archivos actuales de guardado."
                   : $"Are you sure you want to restore this backup?\n\n" +
                     $"Slot: {selected.SlotId}\n" +
                     $"Date: {selected.Timestamp}\n" +
                     $"Files: {selected.FileCount}\n\n" +
                     $"⚠️ WARNING: This will overwrite your current save files.",
                es ? "Confirmar restauración" : "Confirm restore",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // Copiar archivos del backup a la carpeta saves
                foreach (var backupFile in selected.Files)
                {
                    // Extraer el nombre original: "2025-01-23_14-30-00_Slot_00000001.save" -> "Slot_00000001.save"
                    string fileName = Path.GetFileName(backupFile);
                    string originalName = fileName.Substring(fileName.IndexOf(selected.SlotId));
                    string destPath = Path.Combine(_savesFolder, originalName);

                    File.Copy(backupFile, destPath, overwrite: true);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    es ? $"Error al restaurar:\n{ex.Message}"
                       : $"Error restoring:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}