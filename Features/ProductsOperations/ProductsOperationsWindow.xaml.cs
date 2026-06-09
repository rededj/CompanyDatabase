using BDcource.Helpers;
using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace BDcource.Features.ProductsOperations
{
    public partial class ProductOperationsWindow : Window
    {
        private readonly int _productId;
        private readonly string _productName;
        private readonly ProductsOperationsService _service;
        private readonly CourceContext _context;

        public ProductOperationsWindow(int productId, string productName)
        {
            InitializeComponent();
            _productId = productId;
            _productName = productName;
            _context = new CourceContext();
            _service = new ProductsOperationsService(_context);
            txtProductName.Text = _productName;
            LoadOperations();
        }

        private void LoadOperations()
        {
            var ops = _service.GetOperationsForProduct(_productId);
            dgOperations.ItemsSource = ops;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var existingOpIds = _service.GetOperationsForProduct(_productId).Select(o => o.OperationId).ToList();
            var availableOps = _context.Operations
                .Where(o => !existingOpIds.Contains(o.OperationId))
                .Include(o => o.Workshop)
                .ToList();
            if (!availableOps.Any())
            {
                CustomMessageBox.Show("Нет доступных операций для добавления.", "Информация");
                return;
            }

            var selectDialog = new OperationSelectDialog(availableOps);
            if (selectDialog.ShowDialog() == true)
            {
                string error = _service.AddLink(_productId, selectDialog.SelectedOperationId);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadOperations();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgOperations.SelectedItem as Operation;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите операцию для удаления.");
                return;
            }
            if (CustomMessageBox.Show($"Удалить операцию '{selected.Description}' из продукта?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _service.RemoveLink(_productId, selected.OperationId);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadOperations();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}