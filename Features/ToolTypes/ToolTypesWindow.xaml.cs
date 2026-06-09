using System.Windows;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.ToolTypes
{
    public partial class ToolTypesWindow : Window
    {
        private readonly ToolTypeService _toolTypeService;

        public ToolTypesWindow()
        {
            InitializeComponent();
            var context = new CourceContext();
            _toolTypeService = new ToolTypeService(context);
            LoadToolTypes();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadToolTypes()
        {
            dgToolTypes.ItemsSource = _toolTypeService.GetAllToolTypes();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ToolTypeEditDialog();
            if (dialog.ShowDialog() == true)
            {
                _toolTypeService.AddToolType(dialog.Name, dialog.Description);
                LoadToolTypes();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgToolTypes.SelectedItem as ToolType;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите тип инструмента", "Ошибка");
                return;
            }
            var dialog = new ToolTypeEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                _toolTypeService.UpdateToolType(selected.ToolTypeId, dialog.Name, dialog.Description);
                LoadToolTypes();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgToolTypes.SelectedItem as ToolType;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите тип инструмента", "Ошибка");
                return;
            }
            if (CustomMessageBox.Show($"Удалить тип инструмента {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var error = _toolTypeService.DeleteToolType(selected.ToolTypeId);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadToolTypes();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}