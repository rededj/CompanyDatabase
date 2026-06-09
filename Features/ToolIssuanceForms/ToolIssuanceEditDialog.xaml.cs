using System;
using System.Linq;
using System.Windows;
using BDcource.Models;
using BDcource.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;

namespace BDcource.Features.ToolIssuanceForms
{
    public partial class ToolIssuanceEditDialog : Window
    {
        public int WorkOrderId { get; private set; }
        public int OperationId { get; private set; }
        public string SerialNumber { get; private set; }
        public int WorkshopId { get; private set; }
        public int UserId { get; private set; }
        public DateOnly PlannedReturnDate { get; private set; }

        private readonly Models.ToolIssuance _editIssuance;
        private readonly CourceContext _context;
        private readonly bool _isEmployee;

        public ToolIssuanceEditDialog(Models.ToolIssuance editIssuance = null, bool isEmployee = false)
        {
            InitializeComponent();
            _context = new CourceContext();
            _editIssuance = editIssuance;
            _isEmployee = isEmployee;

            LoadComboBoxes();

            if (editIssuance != null)
            {
                cbWorkOrder.SelectedValue = editIssuance.WorkOrderId;
                cbOperation.SelectedValue = editIssuance.OperationId;
                cbTool.SelectedValue = editIssuance.SerialNumber;
                cbWorkshop.SelectedValue = editIssuance.WorkshopId;
                cbUser.SelectedValue = editIssuance.UserId;
                dpPlannedReturnDate.SelectedDate = editIssuance.ReturnDateTime.ToDateTime(TimeOnly.MinValue);
                if (_isEmployee) cbUser.IsEnabled = false;
                cbWorkOrder.IsEnabled = false;
                cbWorkOrder.SelectionChanged += CbWorkOrder_SelectionChanged;
                CbWorkOrder_SelectionChanged(null, null);
            }
            else
            {
                if (RoleHelper.IsEmployee(App.CurrentUser))
                {
                    cbUser.SelectedValue = App.CurrentUser.UserId;
                    cbUser.IsEnabled = false;
                }
                cbWorkOrder.SelectionChanged += CbWorkOrder_SelectionChanged;
            }
        }

        private void LoadComboBoxes()
        {
            if (_editIssuance == null)
            {
                cbWorkOrder.ItemsSource = _context.WorkOrders
                    .Where(wo => wo.Completed == false)
                    .Include(wo => wo.Product)
                    .ToList();

                cbTool.ItemsSource = _context.Tools
                    .Where(t => t.CurrentWorkOrderId == null)
                    .Include(t => t.ToolType)
                    .ToList();
            }
            else
            {
                cbWorkOrder.ItemsSource = _context.WorkOrders
                    .Include(wo => wo.Product)
                    .ToList();
                cbTool.ItemsSource = _context.Tools
                    .Include(t => t.ToolType)
                    .ToList();
            }

            cbWorkshop.ItemsSource = _context.Workshops.ToList();
            cbUser.ItemsSource = _context.Users.ToList();
        }

        private void CbWorkOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbWorkOrder.SelectedValue != null)
            {
                int workOrderId = (int)cbWorkOrder.SelectedValue;
                var workOrder = _context.WorkOrders
                    .Include(wo => wo.Product)
                    .FirstOrDefault(wo => wo.WorkOrderId == workOrderId);
                if (workOrder != null)
                {
                    if (workOrder.Completed && _editIssuance == null)
                    {
                        CustomMessageBox.Show("Нельзя выдавать инструменты в уже выполненный наряд.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        cbWorkOrder.SelectedValue = null;
                        cbOperation.ItemsSource = null;
                        return;
                    }

                    var productId = workOrder.ProductId;
                    var availableOperations = _context.ProductsOperations
                        .Where(po => po.ProductId == productId)
                        .Include(po => po.Operation)
                            .ThenInclude(o => o.Workshop)
                        .Select(po => po.Operation)
                        .ToList();
                    cbOperation.ItemsSource = availableOperations;
                    cbOperation.DisplayMemberPath = "Description";
                    cbOperation.SelectedValuePath = "OperationId";
                }
                else
                {
                    cbOperation.ItemsSource = null;
                }
            }
            else
            {
                cbOperation.ItemsSource = null;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cbWorkOrder.SelectedValue == null) { CustomMessageBox.Show("Выберите наряд"); return; }
            if (cbOperation.SelectedValue == null) { CustomMessageBox.Show("Выберите операцию"); return; }
            if (cbTool.SelectedValue == null) { CustomMessageBox.Show("Выберите инструмент"); return; }
            if (cbWorkshop.SelectedValue == null) { CustomMessageBox.Show("Выберите цех"); return; }
            if (cbUser.SelectedValue == null) { CustomMessageBox.Show("Выберите сотрудника"); return; }
            if (dpPlannedReturnDate.SelectedDate == null) { CustomMessageBox.Show("Выберите плановую дату возврата"); return; }

            WorkOrderId = (int)cbWorkOrder.SelectedValue;
            OperationId = (int)cbOperation.SelectedValue;
            SerialNumber = cbTool.SelectedValue.ToString();
            WorkshopId = (int)cbWorkshop.SelectedValue;
            UserId = (int)cbUser.SelectedValue;
            PlannedReturnDate = DateOnly.FromDateTime(dpPlannedReturnDate.SelectedDate.Value);

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}