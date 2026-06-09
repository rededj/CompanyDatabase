using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.Users
{
    public partial class UserManagementControl : UserControl
    {
        private readonly UserService _userService;
        private readonly CourceContext _context;

        public UserManagementControl()
        {
            InitializeComponent();
            _context = new CourceContext();
            _userService = new UserService(_context);
            LoadUsers();
        }

        private void LoadUsers()
        {
            dgUsers.ItemsSource = _userService.GetAllUsers();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var roles = _userService.GetAllRoles();
            var workshops = _context.Workshops.ToList();
            var dialog = new UserEditDialog(roles, workshops);
            if (dialog.ShowDialog() == true)
            {
                _userService.AddUser(dialog.Login, dialog.Password, dialog.RoleId, dialog.Name, dialog.Position, dialog.WorkshopName);
                LoadUsers();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgUsers.SelectedItem as User;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите пользователя", "Ошибка");
                return;
            }
            var roles = _userService.GetAllRoles();
            var workshops = _context.Workshops.ToList();
            var dialog = new UserEditDialog(roles, workshops, selected);
            if (dialog.ShowDialog() == true)
            {
                _userService.UpdateUser(selected.UserId, dialog.RoleId, dialog.Password, dialog.Name, dialog.Position, dialog.WorkshopName);
                LoadUsers();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgUsers.SelectedItem as User;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите пользователя", "Ошибка");
                return;
            }
            if (selected.UserId == App.CurrentUser.UserId)
            {
                CustomMessageBox.Show("Нельзя удалить самого себя", "Ошибка");
                return;
            }
            if (CustomMessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _userService.DeleteUser(selected.UserId);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadUsers();
            }
        }
    }
}