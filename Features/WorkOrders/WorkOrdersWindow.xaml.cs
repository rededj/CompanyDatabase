using System;
using System.Windows;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.WorkOrders
{
    public partial class WorkOrdersWindow : Window
    {
        private readonly WorkOrderService _workOrderService;

        public WorkOrdersWindow()
        {
            InitializeComponent();
            var context = new CourceContext();
            _workOrderService = new WorkOrderService(context);
            LoadWorkOrders();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadWorkOrders()
        {
            var list = _workOrderService.GetAllWorkOrders().OrderBy(wo => wo.RegistrationDate).ToList();
            dgWorkOrders.ItemsSource = list;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WorkOrderEditDialog();
            if (dialog.ShowDialog() == true)
            {
                _workOrderService.AddWorkOrder(dialog.ProductId, dialog.RegistrationDate, dialog.DueDate, dialog.RequiredQuantity, dialog.Completed);
                LoadWorkOrders();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgWorkOrders.SelectedItem as WorkOrder;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите наряд", "Ошибка");
                return;
            }
            var dialog = new WorkOrderEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                _workOrderService.UpdateWorkOrder(selected.WorkOrderId, dialog.ProductId, dialog.RegistrationDate, dialog.DueDate, dialog.RequiredQuantity, dialog.Completed);
                LoadWorkOrders();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgWorkOrders.SelectedItem as WorkOrder;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите наряд", "Ошибка");
                return;
            }

            string error = _workOrderService.DeleteWorkOrder(selected.WorkOrderId);
            if (error != null)
                CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                LoadWorkOrders();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}