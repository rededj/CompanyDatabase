using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.Operations
{
    public partial class ToolNormEditDialog : Window
    {
        public int ToolTypeId { get; private set; }
        public short QuantityInUse { get; private set; }

        private readonly OperationToolsUsage _editNorm;

        public ToolNormEditDialog(List<ToolType> availableToolTypes)
        {
            InitializeComponent();
            cbToolType.ItemsSource = availableToolTypes;
        }

        public ToolNormEditDialog(OperationToolsUsage norm)
        {
            InitializeComponent();
            _editNorm = norm;
            cbToolType.ItemsSource = new List<ToolType> { norm.ToolType };
            cbToolType.SelectedValue = norm.ToolTypeId;
            cbToolType.IsEnabled = false;
            txtQuantity.Text = norm.QuantityInUse.ToString();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cbToolType.SelectedValue == null) { CustomMessageBox.Show("Выберите тип инструмента"); return; }
            if (!short.TryParse(txtQuantity.Text, out short qty) || qty <= 0) { CustomMessageBox.Show("Количество должно быть положительным числом"); return; }
            ToolTypeId = (int)cbToolType.SelectedValue;
            QuantityInUse = qty;
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