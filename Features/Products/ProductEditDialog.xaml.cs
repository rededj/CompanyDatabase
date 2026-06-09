using System;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource
{
    public partial class ProductEditDialog : Window
    {
        public string Name { get; private set; }
        public decimal Cost { get; private set; }
        public byte OperationsRequired { get; private set; }

        private readonly Product _editProduct;

        public ProductEditDialog(Product editProduct = null)
        {
            InitializeComponent();
            _editProduct = editProduct;

            if (editProduct != null)
            {
                txtName.Text = editProduct.Name;
                txtCost.Text = editProduct.Cost.ToString();
                txtOperationsRequired.Text = editProduct.OperationsRequired.ToString();
                txtOperationsRequired.IsReadOnly = true;
                txtOperationsRequired.ToolTip = "Изменяется автоматически";
                Header.Text = "Редактирование продукта";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                CustomMessageBox.Show("Введите название", "Ошибка");
                return;
            }
            if (!decimal.TryParse(txtCost.Text, out decimal cost) || cost < 1)
            {
                CustomMessageBox.Show("Стоимость должна быть числом >= 1", "Ошибка");
                return;
            }

            Name = txtName.Text.Trim();
            Cost = cost;

            if (_editProduct == null)
            {
                OperationsRequired = 0;
            }
            else
            {
                OperationsRequired = _editProduct.OperationsRequired;
            }

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