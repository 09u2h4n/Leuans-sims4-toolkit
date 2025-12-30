using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace ModernDesign.MVVM.View
{
    public partial class DLCUnlockerWindow : Window
    {
        public DLCUnlockerWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => RefreshUnlockerStatus();
        }

        // ============================================================
        // REFRESH STATUS
        // ============================================================

        private async void RefreshUnlockerStatus()
        {
            try
            {
                if (UnlockerService.IsUnlockerInstalled(out var clientName))
                {
                    StatusText.Text = $"DLC Unlocker: Installed ({clientName})";
                    StatusText.Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#22C55E"));

                    HintText.Text =
                        "The unlocker is correctly installed.\n" +
                        "You can now open EA app / Origin and play normally.";

                    InstallBtn.Content = "Reinstall EA DLC Unlocker";
                    UninstallBtn.Visibility = Visibility.Visible;

                    // Check if configuration file is up to date
                    await CheckConfigurationFile();
                }
                else
                {
                    StatusText.Text = "DLC Unlocker: Not installed";
                    StatusText.Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#F97373"));

                    HintText.Text =
                        "Click \"Install EA DLC Unlocker\" to automatically download,\n" +
                        "extract, and configure everything.";

                    InstallBtn.Content = "Install EA DLC Unlocker";
                    UninstallBtn.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                StatusText.Text = "DLC Unlocker: Status unknown";
                StatusText.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FBBF24"));

                HintText.Text =
                    "Could not determine unlocker status.\n" +
                    "You may still attempt to install it.";

                UninstallBtn.Visibility = Visibility.Collapsed;
            }
        }

        private async Task CheckConfigurationFile()
        {
            try
            {
                string configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "anadius", "EA DLC Unlocker v2", "g_The Sims 4.ini");

                if (!File.Exists(configPath))
                    return;

                string localContent = File.ReadAllText(configPath);

                using (var httpClient = new HttpClient())
                {
                    string remoteContent = await httpClient.GetStringAsync(
                        "https://zeroauno.blob.core.windows.net/leuan/Public/links.ini?sp=r&st=2025-12-29T23:45:43Z&se=2026-02-28T08:00:43Z&spr=https&sv=2024-11-04&sr=b&sig=8BPnZQivztM%2FNt88BVh%2F1ZMKlhP4u8HzWbCfXXCZcy0%3D");

                    if (localContent.Trim() == remoteContent.Trim())
                        return;

                    // Configuration is outdated
                    var result = MessageBox.Show(
                        "We've detected that your EA DLC Unlocker is outdated and you're missing some DLCs!\n" +
                        "Would you like to know which DLCs you're missing?\n\n",
                        "Outdated Configuration",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        var missingDLCs = GetMissingDLCs(localContent, remoteContent);
                        await ShowMissingDLCsDialog(missingDLCs, configPath, remoteContent);
                    }
                }
            }
            catch
            {
                // Silently fail - don't interrupt the user experience
            }
        }

        private List<string> GetMissingDLCs(string localContent, string remoteContent)
        {
            var localDLCs = ParseDLCNames(localContent);
            var remoteDLCs = ParseDLCNames(remoteContent);

            return remoteDLCs.Except(localDLCs).ToList();
        }

        private List<string> ParseDLCNames(string content)
        {
            var dlcs = new List<string>();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("DLC_") || line.Contains("="))
                {
                    var parts = line.Split('=');
                    if (parts.Length > 0)
                    {
                        var dlcName = parts[0].Trim();
                        if (dlcName.StartsWith("DLC_"))
                            dlcs.Add(dlcName);
                    }
                }
            }

            return dlcs;
        }
        private async Task ShowMissingDLCsDialog(List<string> missingDLCs, string configPath, string remoteContent)
        {
            string dlcList = string.Join("\n", missingDLCs.Select(d => "• " + d.Replace("DLC_", "").Replace("_", " ")));

            var updateResult = MessageBox.Show(
                "Would you like to update your EA DLC Unlocker to include the missing DLCs?\n",
                "Update Configuration",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (updateResult == MessageBoxResult.Yes)
            {
                try
                {
                    File.WriteAllText(configPath, remoteContent);

                    MessageBox.Show(
                        "Configuration updated successfully! Please restart EA app/Origin.\n",
                        "Success / Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to update configuration:\n{ex.Message}\n\n" +
                        $"Error al actualizar la configuración:\n{ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
        // ============================================================
        // DELAYED SECOND REFRESH (5 SECONDS)
        // ============================================================

        private void ScheduleDelayedRefresh()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            timer.Tick += (s, e) =>
            {
                ((DispatcherTimer)s).Stop();
                RefreshUnlockerStatus();
            };

            timer.Start();
        }

        // ============================================================
        // INSTALL
        // ============================================================

        private async void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            InstallBtn.IsEnabled = false;
            UninstallBtn.IsEnabled = false;

            StatusText.Text = "Installing EA DLC Unlocker...";
            StatusText.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#60A5FA"));
            HintText.Text = "Downloading and configuring components. Please wait maximum 1 minute...";

            try
            {
                await UnlockerService.InstallUnlockerAsync();

                MessageBox.Show(
                    "EA DLC Unlocker has been installed.\n",
                    "Unlocker Installed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                RefreshUnlockerStatus();
                ScheduleDelayedRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not install/configure EA DLC Unlocker:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                StatusText.Text = "Error while installing unlocker";
                StatusText.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#EF4444"));
                HintText.Text =
                    "Verify your internet connection, permissions, or sims 4 installation.";
            }
            finally
            {
                InstallBtn.IsEnabled = true;
                UninstallBtn.IsEnabled = true;
            }
        }

        // ============================================================
        // UNINSTALL
        // ============================================================

        private async void UninstallBtn_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show(
                "Are you sure you want to uninstall the EA DLC Unlocker?\n\n" +
                "version.dll, configs and local unlocker files will be removed.",
                "Confirm Uninstall",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            InstallBtn.IsEnabled = false;
            UninstallBtn.IsEnabled = false;

            StatusText.Text = "Uninstalling EA DLC Unlocker...";
            StatusText.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#F97373"));
            HintText.Text = "Removing unlocker files and configurations...";

            try
            {
                await UnlockerService.UninstallUnlockerAsync();

                MessageBox.Show(
                    "EA DLC Unlocker files and configs have been removed.",
                    "Unlocker Uninstalled",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                RefreshUnlockerStatus();
                ScheduleDelayedRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Some parts of the unlocker could not be removed:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                StatusText.Text = "Error while uninstalling unlocker";
                StatusText.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#EF4444"));
                HintText.Text =
                    "Make sure EA/Origin are closed and try again.";
            }
            finally
            {
                InstallBtn.IsEnabled = true;
                UninstallBtn.IsEnabled = true;
            }
        }

        // ============================================================
        // NAVIGATION
        // ============================================================

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            var fadeOut = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(200) };

            fadeOut.Completed += (s, args) =>
            {
                this.Hide();
                mainWindow.Opacity = 0;
                mainWindow.Show();

                var fadeIn = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(200) };
                fadeIn.Completed += (s2, args2) => this.Close();
                mainWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void OtherOS_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = e.Uri.AbsoluteUri,
                UseShellExecute = false
            });
            e.Handled = true;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            var fadeOut = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(200) };

            fadeOut.Completed += (s, args) =>
            {
                this.Hide();
                mainWindow.Opacity = 0;
                mainWindow.Show();

                var fadeIn = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(200) };
                fadeIn.Completed += (s2, args2) => this.Close();
                mainWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }
    }
}
