using System;
using System.Windows;
using System.Windows.Media;

namespace ModernDesign.MVVM.View
{
    public partial class ColorPickerWindow : Window
    {
        public string Color1 { get; private set; }
        public string Color2 { get; private set; }
        public string Color3 { get; private set; }

        private readonly string[] _defaultColors = { "#22D3EE", "#1E293B", "#21b96b" };

        public ColorPickerWindow(string[] currentColors)
        {
            InitializeComponent();

            // Inicializar valores por defecto
            Color1 = "#22D3EE";
            Color2 = "#1E293B";
            Color3 = "#21b96b";

            // Cargar colores actuales
            if (currentColors != null && currentColors.Length == 3)
            {
                LoadColorToSliders(currentColors[0], R1Slider, G1Slider, B1Slider);
                LoadColorToSliders(currentColors[1], R2Slider, G2Slider, B2Slider);
                LoadColorToSliders(currentColors[2], R3Slider, G3Slider, B3Slider);
            }
            else
            {
                // Cargar valores por defecto
                LoadColorToSliders(_defaultColors[0], R1Slider, G1Slider, B1Slider);
                LoadColorToSliders(_defaultColors[1], R2Slider, G2Slider, B2Slider);
                LoadColorToSliders(_defaultColors[2], R3Slider, G3Slider, B3Slider);
            }
        }

        private void LoadColorToSliders(string hexColor, System.Windows.Controls.Slider rSlider,
                                        System.Windows.Controls.Slider gSlider, System.Windows.Controls.Slider bSlider)
        {
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(hexColor);
                rSlider.Value = color.R;
                gSlider.Value = color.G;
                bSlider.Value = color.B;
            }
            catch { }
        }

        private void Color1_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (Preview1 == null || Hex1Text == null || FinalGrad1 == null) return;

                byte r = (byte)R1Slider.Value;
                byte g = (byte)G1Slider.Value;
                byte b = (byte)B1Slider.Value;

                Color color = Color.FromRgb(r, g, b);
                Preview1.Background = new SolidColorBrush(color);
                Hex1Text.Text = $"#{r:X2}{g:X2}{b:X2}";
                Color1 = Hex1Text.Text;

                FinalGrad1.Color = color;
            }
            catch { }
        }

        private void Color2_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (Preview2 == null || Hex2Text == null || FinalGrad2 == null) return;

                byte r = (byte)R2Slider.Value;
                byte g = (byte)G2Slider.Value;
                byte b = (byte)B2Slider.Value;

                Color color = Color.FromRgb(r, g, b);
                Preview2.Background = new SolidColorBrush(color);
                Hex2Text.Text = $"#{r:X2}{g:X2}{b:X2}";
                Color2 = Hex2Text.Text;

                FinalGrad2.Color = color;
            }
            catch { }
        }

        private void Color3_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (Preview3 == null || Hex3Text == null || FinalGrad3 == null) return;

                byte r = (byte)R3Slider.Value;
                byte g = (byte)G3Slider.Value;
                byte b = (byte)B3Slider.Value;

                Color color = Color.FromRgb(r, g, b);
                Preview3.Background = new SolidColorBrush(color);
                Hex3Text.Text = $"#{r:X2}{g:X2}{b:X2}";
                Color3 = Hex3Text.Text;

                FinalGrad3.Color = color;
            }
            catch { }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            LoadColorToSliders(_defaultColors[0], R1Slider, G1Slider, B1Slider);
            LoadColorToSliders(_defaultColors[1], R2Slider, G2Slider, B2Slider);
            LoadColorToSliders(_defaultColors[2], R3Slider, G3Slider, B3Slider);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}