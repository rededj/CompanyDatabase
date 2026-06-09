using BDcource.Helpers;
using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BDcource.Features.MaterialIssuanceForms
{
    public partial class MaterialIssuanceEditDialog : Window
    {
        public int WorkOrderId { get; private set; }
        public int OperationId { get; private set; }
        public int UserId { get; private set; }
        public int MaterialId { get; private set; }
        public int ActualQuantity { get; private set; }
        public DateOnly IssueDate { get; private set; }

        private readonly bool _isEditMode;
        private readonly int _existingIssuanceId;
        private readonly CourceContext _context;

        public MaterialIssuanceEditDialog()
        {
            InitializeComponent();
            _isEditMode = false;
            _context = new CourceContext();
            LoadComboBoxes();
            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                cbUser.SelectedValue = App.CurrentUser.UserId;
                cbUser.IsEnabled = false;
            }
            cbWorkOrder.SelectionChanged += CbWorkOrder_SelectionChanged;
        }

        public MaterialIssuanceEditDialog(MaterialIssuance issuance)
        {
            InitializeComponent();
            _isEditMode = true;
            _existingIssuanceId = issuance.IssuanceId;
            _context = new CourceContext();
            LoadComboBoxes();
            cbWorkOrder.SelectedValue = issuance.WorkOrderId;
            cbOperation.SelectedValue = issuance.OperationId;
            cbUser.SelectedValue = issuance.UserId;
            txtQuantity.Text = issuance.ActualQuantity.ToString();
            dpIssueDate.SelectedDate = issuance.IssueDateTime;
            cbMaterial.Visibility = Visibility.Collapsed;
            lblMaterial.Visibility = Visibility.Collapsed;
            cbWorkOrder.SelectionChanged += CbWorkOrder_SelectionChanged;
            CbWorkOrder_SelectionChanged(null, null);
        }

        private void LoadComboBoxes()
        {
            cbWorkOrder.ItemsSource = _context.WorkOrders.Include(wo => wo.Product).ToList();
            cbUser.ItemsSource = _context.Users.ToList();
            cbMaterial.ItemsSource = _context.Materials.ToList();
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
            if (cbUser.SelectedValue == null) { CustomMessageBox.Show("Выберите сотрудника"); return; }
            if (!_isEditMode && cbMaterial.SelectedValue == null) { CustomMessageBox.Show("Выберите материал"); return; }
            if (!int.TryParse(txtQuantity.Text, out int qty) || qty <= 0) { CustomMessageBox.Show("Количество должно быть положительным"); return; }
            if (dpIssueDate.SelectedDate == null) { CustomMessageBox.Show("Выберите дату"); return; }

            WorkOrderId = (int)cbWorkOrder.SelectedValue;
            OperationId = (int)cbOperation.SelectedValue;
            UserId = (int)cbUser.SelectedValue;
            if (!_isEditMode) MaterialId = (int)cbMaterial.SelectedValue;
            ActualQuantity = qty;
            IssueDate = DateOnly.FromDateTime(dpIssueDate.SelectedDate.Value);

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