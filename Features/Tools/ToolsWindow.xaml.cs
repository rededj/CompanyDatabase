using System.Windows;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.Tools
{
    public partial class ToolsWindow : Window
    {
        private readonly ToolService _toolService;

        public ToolsWindow()
        {
            InitializeComponent();
            var context = new CourceContext();
            _toolService = new ToolService(context);
            LoadTools();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadTools()
        {
            dgTools.ItemsSource = _toolService.GetAllTools();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ToolEditDialog();
            if (dialog.ShowDialog() == true)
            {
                string error = _toolService.AddTool(dialog.SerialNumber, dialog.ToolTypeId, dialog.ArrivalDate);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadTools();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgTools.SelectedItem as Tool;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите инструмент", "Ошибка");
                return;
            }
            var dialog = new ToolEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                string error = _toolService.UpdateTool(selected.SerialNumber, dialog.ArrivalDate);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadTools();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgTools.SelectedItem as Tool;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите инструмент", "Ошибка");
                return;
            }
            if (CustomMessageBox.Show($"Удалить инструмент {selected.SerialNumber}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var error = _toolService.DeleteTool(selected.SerialNumber);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadTools();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}