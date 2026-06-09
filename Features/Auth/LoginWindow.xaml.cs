using BDcource.Models;
using BDcource.Features.Auth;
using System;
using System.Windows;

namespace BDcource
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                tbError.Text = "Введите логин и пароль";
                return;
            }

            using (var context = new CourceContext())
            {
                var authService = new AuthService(context);
                var user = authService.Authenticate(login, password);
                if (user == null)
                {
                    tbError.Text = "Неверный логин или пароль";
                    return;
                }
                context.Entry(user).Reference(u => u.Role).Load();
                App.CurrentUser = user;

                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
    }
}