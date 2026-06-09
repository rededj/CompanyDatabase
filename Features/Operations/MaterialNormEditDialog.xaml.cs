using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.Operations
{
    public partial class MaterialNormEditDialog : Window
    {
        public int MaterialId { get; private set; }
        public short RequiredQuantity { get; private set; }

        private readonly OperationMaterialsUsage _editNorm;

        public MaterialNormEditDialog(List<Material> availableMaterials)
        {
            InitializeComponent();
            cbMaterial.ItemsSource = availableMaterials;
        }

        public MaterialNormEditDialog(OperationMaterialsUsage norm)
        {
            InitializeComponent();
            _editNorm = norm;
            cbMaterial.ItemsSource = new List<Material> { norm.Material };
            cbMaterial.SelectedValue = norm.MaterialId;
            cbMaterial.IsEnabled = false;
            txtQuantity.Text = norm.RequiredQuantity.ToString();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cbMaterial.SelectedValue == null) { CustomMessageBox.Show("Выберите материал"); return; }
            if (!short.TryParse(txtQuantity.Text, out short qty) || qty <= 0) { CustomMessageBox.Show("Количество должно быть положительным числом"); return; }
            MaterialId = (int)cbMaterial.SelectedValue;
            RequiredQuantity = qty;
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