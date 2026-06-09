using BDcource.Features.Products;
using BDcource.Features.ProductsOperations;
using BDcource.Helpers;
using BDcource.Models;
using System;
using System.Windows;

namespace BDcource
{
    public partial class ProductsWindow : Window
    {
        public ProductsWindow()
        {
            InitializeComponent();
            LoadProducts();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadProducts()
        {
            using (var context = new CourceContext())
            {
                var productService = new ProductService(context);
                dgProducts.ItemsSource = productService.GetAllProducts();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductEditDialog();
            if (dialog.ShowDialog() == true)
            {
                using (var context = new CourceContext())
                {
                    var productService = new ProductService(context);
                    productService.AddProduct(dialog.Name, dialog.Cost, dialog.OperationsRequired);
                }
                LoadProducts();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите продукт", "Ошибка");
                return;
            }
            var dialog = new ProductEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                using (var context = new CourceContext())
                {
                    var productService = new ProductService(context);
                    productService.UpdateProduct(selected.ProductId, dialog.Name, dialog.Cost, dialog.OperationsRequired);
                }
                LoadProducts();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProducts.SelectedItem as Product;
            if (selected == null) { CustomMessageBox.Show("Выберите продукт"); return; }
            if (CustomMessageBox.Show($"Удалить продукт {selected.Name}?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            using (var context = new CourceContext())
            {
                var productService = new ProductService(context);
                string error = productService.DeleteProduct(selected.ProductId);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadProducts();
            }
        }

        private void BtnOperations_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите продукт.");
                return;
            }
            var win = new ProductOperationsWindow(selected.ProductId, selected.Name);
            win.Owner = this;
            win.ShowDialog();
            LoadProducts(); 
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}