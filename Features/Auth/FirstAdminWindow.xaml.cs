using BDcource.Models;
using BDcource.Features.Auth;
using System;
using System.Windows;
using BDcource.Helpers;

namespace BDcource
{
    public partial class FirstAdminWindow : Window
    {
        public FirstAdminWindow()
        {
            InitializeComponent();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;
            string confirm = txtConfirm.Password;
            string name = txtName.Text.Trim();
            string position = txtPosition.Text.Trim();
            string workshopName = txtWorkshopName.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                tbError.Text = "Введите логин и пароль";
                return;
            }
            if (password != confirm)
            {
                tbError.Text = "Пароли не совпадают";
                return;
            }
            if (string.IsNullOrEmpty(name))
            {
                tbError.Text = "Введите ФИО";
                return;
            }

            using (var context = new CourceContext())
            {
                var authService = new AuthService(context);
                try
                {
                    authService.CreateFirstAdmin(login, password, name, position, workshopName);
                    CustomMessageBox.Show("Администратор создан. Теперь можно войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    tbError.Text = ex.Message;
                }
            }
        }
    }
}