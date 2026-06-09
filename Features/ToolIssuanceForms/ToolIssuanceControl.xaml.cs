using System;
using System.Windows;
using System.Windows.Controls;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.ToolIssuanceForms
{
    public partial class ToolIssuanceControl : UserControl
    {
        private readonly ToolIssuanceService _service;
        private readonly CourceContext _context;

        public ToolIssuanceControl()
        {
            InitializeComponent();
            _context = new CourceContext();
            _service = new ToolIssuanceService(_context);
            LoadIssuances();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnEdit.Visibility = Visibility.Collapsed;
                btnReturn.Visibility = Visibility.Collapsed;
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
            var dialog = new ToolIssuanceEditDialog();
            if (dialog.ShowDialog() == true)
            {
                string error = _service.AddIssuance(dialog.WorkOrderId, dialog.OperationId, dialog.SerialNumber, dialog.WorkshopId, dialog.UserId, DateOnly.FromDateTime(DateTime.Now), dialog.PlannedReturnDate);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadIssuances();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgIssuances.SelectedItem as ToolIssuance;
            if (selected == null) { CustomMessageBox.Show("Выберите выдачу"); return; }
            if (selected.ActualReturnDate != null) { CustomMessageBox.Show("Нельзя редактировать уже возвращённую выдачу"); return; }
            var dialog = new ToolIssuanceEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                string error = _service.UpdateIssuance(selected.IssuanceId, dialog.WorkOrderId, dialog.OperationId, dialog.WorkshopId, dialog.PlannedReturnDate);
                if (error != null) CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else LoadIssuances();
            }
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgIssuances.SelectedItem as ToolIssuance;
            if (selected == null) { CustomMessageBox.Show("Выберите выдачу"); return; }
            if (selected.ActualReturnDate != null) { CustomMessageBox.Show("Инструмент уже возвращён"); return; }
            if (CustomMessageBox.Show("Вернуть инструмент? Будет установлена фактическая дата возврата.", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _service.ReturnTool(selected.IssuanceId, DateOnly.FromDateTime(DateTime.Now));
                if (error != null) CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else LoadIssuances();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgIssuances.SelectedItem as ToolIssuance;
            if (selected == null) { CustomMessageBox.Show("Выберите выдачу"); return; }
            var msg = selected.ActualReturnDate == null ?
                "Выдача не возвращена. Удаление вернёт инструмент на склад. Продолжить?" :
                "Удалить запись о возвращённой выдаче?";
            if (CustomMessageBox.Show(msg, "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _service.DeleteIssuance(selected.IssuanceId);
                if (error != null) CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else LoadIssuances();
            }
        }
    }
}