using System;
using System.Linq;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.WorkOrders
{
    public partial class WorkOrderEditDialog : Window
    {
        public int ProductId { get; private set; }
        public DateOnly RegistrationDate { get; private set; }
        public DateOnly DueDate { get; private set; }
        public int RequiredQuantity { get; private set; }
        public bool Completed { get; private set; }

        private readonly WorkOrder _editWorkOrder;
        private readonly CourceContext _context;

        public WorkOrderEditDialog(WorkOrder editWorkOrder = null)
        {
            InitializeComponent();
            _context = new CourceContext();
            _editWorkOrder = editWorkOrder;

            cbProduct.ItemsSource = _context.Products.ToList();

            if (editWorkOrder != null)
            {
                cbProduct.SelectedValue = editWorkOrder.ProductId;
                dpRegistrationDate.SelectedDate = editWorkOrder.RegistrationDate.ToDateTime(TimeOnly.MinValue);
                dpDueDate.SelectedDate = editWorkOrder.DueDate.ToDateTime(TimeOnly.MinValue);
                txtRequiredQuantity.Text = editWorkOrder.RequiredQuantity.ToString();
                chkCompleted.IsChecked = editWorkOrder.Completed;
                Header.Text = "Редактирование наряда";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cbProduct.SelectedValue == null)
            {
                CustomMessageBox.Show("Выберите продукт", "Ошибка");
                return;
            }
            if (dpRegistrationDate.SelectedDate == null)
            {
                CustomMessageBox.Show("Выберите дату регистрации", "Ошибка");
                return;
            }
            if (dpDueDate.SelectedDate == null)
            {
                CustomMessageBox.Show("Выберите срок выполнения", "Ошибка");
                return;
            }
            if (!int.TryParse(txtRequiredQuantity.Text, out int quantity) || quantity <= 0)
            {
                CustomMessageBox.Show("Количество должно быть целым положительным числом", "Ошибка");
                return;
            }

            ProductId = (int)cbProduct.SelectedValue;
            RegistrationDate = DateOnly.FromDateTime(dpRegistrationDate.SelectedDate.Value);
            DueDate = DateOnly.FromDateTime(dpDueDate.SelectedDate.Value);
            RequiredQuantity = quantity;
            Completed = chkCompleted.IsChecked == true;

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