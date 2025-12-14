using System.Windows;
using System.Windows.Input;

namespace ModernDesign.MVVM.View
{
    public partial class AvatarSelectorWindow : Window
    {
        public string SelectedAvatar { get; private set; }

        public AvatarSelectorWindow()
        {
            InitializeComponent();
        }

        private void Avatar_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as System.Windows.Controls.Border;
            if (border != null)
            {
                SelectedAvatar = border.Tag.ToString();
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}