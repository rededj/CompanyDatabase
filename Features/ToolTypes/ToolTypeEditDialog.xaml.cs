using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.ToolTypes
{
    public partial class ToolTypeEditDialog : Window
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        private readonly ToolType _editToolType;

        public ToolTypeEditDialog()
        {
            InitializeComponent();
        }

        public ToolTypeEditDialog(ToolType editToolType)
        {
            InitializeComponent();
            _editToolType = editToolType;
            txtName.Text = editToolType.Name;
            txtDescription.Text = editToolType.Description;
            Header.Text = "Редактирование типа инструмента";
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                CustomMessageBox.Show("Введите наименование", "Ошибка");
                return;
            }
            Name = txtName.Text.Trim();
            Description = txtDescription.Text.Trim();
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