using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource
{
    public partial class MaterialEditDialog : Window
    {
        public string Name { get; private set; }
        public int NumberOf { get; private set; }
        private readonly Material _editMaterial;

        public MaterialEditDialog(Material editMaterial = null)
        {
            InitializeComponent();
            _editMaterial = editMaterial;
            if (editMaterial != null)
            {
                txtName.Text = editMaterial.Name;
                txtNumberOf.Text = editMaterial.NumberOf.ToString();
                Header.Text = "Редактирование материала";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                CustomMessageBox.Show("Введите наименование", "Ошибка");
                return;
            }
            if (!int.TryParse(txtNumberOf.Text, out int numberOf) || numberOf < 0)
            {
                CustomMessageBox.Show("Количество должно быть целым неотрицательным числом", "Ошибка");
                return;
            }
            Name = txtName.Text.Trim();
            NumberOf = numberOf;
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