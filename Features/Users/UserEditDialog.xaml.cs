using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource
{
    public partial class UserEditDialog : Window
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public int RoleId { get; private set; }
        public string Name { get; private set; }
        public string Position { get; private set; }
        public string WorkshopName { get; private set; }

        private readonly List<Role> _roles;
        private readonly List<Workshop> _workshops;
        private readonly User _editUser;

        public UserEditDialog(List<Role> roles, List<Workshop> workshops, User editUser = null)
        {
            InitializeComponent();
            _roles = roles;
            _workshops = workshops;
            _editUser = editUser;

            cbRole.ItemsSource = _roles;
            var workshopNames = new List<string> { "" };
            workshopNames.AddRange(_workshops.Select(w => w.WorkshopName).Distinct());
            cbWorkshop.ItemsSource = workshopNames;

            if (editUser != null)
            {
                txtLogin.Text = editUser.Login;
                txtLogin.IsEnabled = false;
                cbRole.SelectedValue = editUser.RoleId;
                txtName.Text = editUser.Name;
                txtPosition.Text = editUser.Position;
                cbWorkshop.SelectedItem = editUser.WorkshopName ?? "";
                Header.Text = "Редактирование пользователя";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                CustomMessageBox.Show("Логин обязателен", "Ошибка");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                CustomMessageBox.Show("ФИО обязательно", "Ошибка");
                return;
            }
            Login = txtLogin.Text.Trim();
            Password = txtPassword.Password;
            if (cbRole.SelectedValue == null)
            {
                CustomMessageBox.Show("Выберите роль", "Ошибка");
                return;
            }
            RoleId = (int)cbRole.SelectedValue;
            Name = txtName.Text.Trim();
            Position = txtPosition.Text.Trim();
            string selectedWorkshop = cbWorkshop.SelectedItem?.ToString();
            WorkshopName = string.IsNullOrEmpty(selectedWorkshop) ? null : selectedWorkshop;
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