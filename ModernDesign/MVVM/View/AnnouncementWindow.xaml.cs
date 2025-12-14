using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ModernDesign.MVVM.View
{
    public partial class AnnouncementWindow : Window
    {
        public AnnouncementWindow(string announcementText, string imageUrl = null, string logoUrl = null)
        {
            InitializeComponent();
            this.Loaded += AnnouncementWindow_Loaded;

            // Establecer el texto del anuncio
            AnnouncementTextBlock.Text = announcementText;

            // Cargar imagen si existe
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                try
                {
                    AnnouncementImage.Source = new BitmapImage(new Uri(imageUrl));
                    ImageBorder.Visibility = Visibility.Visible;
                }
                catch
                {
                    // Si falla la carga de imagen, simplemente no la mostramos
                    ImageBorder.Visibility = Visibility.Collapsed;
                }
            }

            // Cargar logo si existe
            if (!string.IsNullOrWhiteSpace(logoUrl))
            {
                try
                {
                    LogoImage.Source = new BitmapImage(new Uri(logoUrl));
                    LogoImage.Visibility = Visibility.Visible;
                }
                catch
                {
                    // Si falla la carga del logo, simplemente no lo mostramos
                    LogoImage.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AnnouncementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            string languageCode = GetLanguageCode();
            bool isSpanish = languageCode.StartsWith("es", StringComparison.OrdinalIgnoreCase);

            if (isSpanish)
            {
                HeaderText.Text = "📢 Anuncio";
                CloseButton.Content = "Cerrar";
            }
            else
            {
                HeaderText.Text = "📢 Announcement";
                CloseButton.Content = "Close";
            }
        }

        private string GetLanguageCode()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string toolkitFolder = Path.Combine(appData, "Leuan's - Sims 4 ToolKit");
            string iniPath = Path.Combine(toolkitFolder, "language.ini");

            string languageCode = "en-US";

            try
            {
                if (File.Exists(iniPath))
                {
                    string[] lines = File.ReadAllLines(iniPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Language = ", StringComparison.OrdinalIgnoreCase))
                        {
                            languageCode = line.Substring("Language = ".Length).Trim();
                            break;
                        }
                    }
                }
            }
            catch
            {
                // Si falla, quedamos con en-US
            }

            return languageCode;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}