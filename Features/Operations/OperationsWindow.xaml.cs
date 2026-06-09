using System.Windows;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.Operations
{
    public partial class OperationsWindow : Window
    {
        private readonly OperationService _operationService;

        public OperationsWindow()
        {
            InitializeComponent();
            var context = new CourceContext();
            _operationService = new OperationService(context);
            LoadOperations();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                btnView.Visibility = Visibility.Visible;
            }
        }

        private void LoadOperations()
        {
            dgOperations.ItemsSource = _operationService.GetAllOperations();
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgOperations.SelectedItem as Operation;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите операцию", "Ошибка");
                return;
            }
            var dialog = new OperationEditDialog(selected, readOnly: true);
            dialog.ShowDialog();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OperationEditDialog();
            if (dialog.ShowDialog() == true)
            {
                _operationService.AddOperation(dialog.WorkshopId, dialog.Description, dialog.AverageDuration, dialog.BlueprintNumber);
                LoadOperations();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgOperations.SelectedItem as Operation;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите операцию", "Ошибка");
                return;
            }
            var dialog = new OperationEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                _operationService.UpdateOperation(selected.OperationId, dialog.WorkshopId, dialog.Description, dialog.AverageDuration, dialog.BlueprintNumber);
                LoadOperations();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgOperations.SelectedItem as Operation;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите операцию", "Ошибка");
                return;
            }
            if (CustomMessageBox.Show($"Удалить операцию '{selected.Description}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _operationService.DeleteOperation(selected.OperationId);
                LoadOperations();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}