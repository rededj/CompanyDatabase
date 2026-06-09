using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.ProductsOperations
{
    public partial class OperationSelectDialog : Window
    {
        public int SelectedOperationId { get; private set; }

        public OperationSelectDialog(List<Operation> availableOperations)
        {
            InitializeComponent();
            dgAvailableOps.ItemsSource = availableOperations;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgAvailableOps.SelectedItem as Operation;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите операцию.");
                return;
            }
            SelectedOperationId = selected.OperationId;
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