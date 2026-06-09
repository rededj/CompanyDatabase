using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource
{
    public partial class WorkshopEditDialog : Window
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        private readonly Workshop _editWorkshop;

        public WorkshopEditDialog(Workshop editWorkshop = null)
        {
            InitializeComponent();
            _editWorkshop = editWorkshop;
            if (editWorkshop != null)
            {
                txtName.Text = editWorkshop.WorkshopName;
                txtAddress.Text = editWorkshop.Adress;
                Header.Text = "Редактирование цеха";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                CustomMessageBox.Show("Введите название цеха", "Ошибка");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                CustomMessageBox.Show("Введите адрес", "Ошибка");
                return;
            }
            Name = txtName.Text.Trim();
            Address = txtAddress.Text.Trim();
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