using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;

namespace BDcource.Helpers
{
    public partial class CustomMessageBoxWindow : Window
    {
        public MessageBoxResult Result { get; private set; }

        public CustomMessageBoxWindow(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            InitializeComponent();
            this.Title = title;
            txtMessage.Text = message;

            switch (icon)
            {
                case MessageBoxImage.Error:
                    iconText.Text = "!";
                    iconText.Foreground = Brushes.White;
                    break;
                case MessageBoxImage.Warning:
                    iconText.Text = "⚠";
                    iconText.Foreground = Brushes.White;
                    break;
                case MessageBoxImage.Question:
                    iconText.Text = "?";
                    iconText.Foreground = Brushes.White;
                    break;
                default:
                    iconText.Text = "i";
                    iconText.Foreground = Brushes.White;
                    break;
            }

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    btnOk.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    btnOk.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    btnOk.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }
    }
}