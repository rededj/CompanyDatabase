using System;
using System.Windows;
using System.Windows.Controls;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.MaterialIssuanceForms
{
    public partial class MaterialIssuanceControl : UserControl
    {
        private readonly MaterialIssuanceService _service;
        private readonly CourceContext _context;

        public MaterialIssuanceControl()
        {
            InitializeComponent();
            _context = new CourceContext();
            _service = new MaterialIssuanceService(_context);
            LoadIssuances();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        public void RefreshData()
        {
            LoadIssuances();
        }

        private void LoadIssuances()
        {
            dgIssuances.ItemsSource = _service.GetAllIssuances();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MaterialIssuanceEditDialog();
            if (dialog.ShowDialog() == true)
            {
                string error = _service.AddIssuance(dialog.WorkOrderId, dialog.OperationId, dialog.UserId, dialog.MaterialId, dialog.ActualQuantity, dialog.IssueDate);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadIssuances();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgIssuances.SelectedItem as Models.MaterialIssuance;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите выдачу", "Ошибка");
                return;
            }
            var dialog = new MaterialIssuanceEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                string error = _service.UpdateIssuance(selected.IssuanceId, dialog.WorkOrderId, dialog.OperationId, dialog.UserId, dialog.ActualQuantity, dialog.IssueDate);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadIssuances();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgIssuances.SelectedItem as Models.MaterialIssuance;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите выдачу", "Ошибка");
                return;
            }
            if (CustomMessageBox.Show($"Удалить выдачу материала {selected.Material?.Name}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _service.DeleteIssuance(selected.IssuanceId);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadIssuances();
            }
        }
    }
}