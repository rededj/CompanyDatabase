using System.Linq;
using System.Windows;

namespace BDcource.Helpers
{
    public static class CustomMessageBox
    {
        public static MessageBoxResult Show(string message, string title = "Сообщение",
                                            MessageBoxButton buttons = MessageBoxButton.OK,
                                            MessageBoxImage icon = MessageBoxImage.Information)
        {
            var window = new CustomMessageBoxWindow(message, title, buttons, icon);
            window.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            window.ShowDialog();
            return window.Result;
        }

        public static MessageBoxResult ShowError(string message, string title = "Ошибка")
        {
            return Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowWarning(string message, string title = "Предупреждение")
        {
            return Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static MessageBoxResult ShowInfo(string message, string title = "Информация")
        {
            return Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult ShowQuestion(string message, string title = "Подтверждение")
        {
            return Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}