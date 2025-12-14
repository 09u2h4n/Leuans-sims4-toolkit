using System.Windows;

namespace ModernDesign.MVVM.View
{
    public partial class MainSelectionWindow : Window
    {
        public MainSelectionWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}