using BDcource.Features.Blueprints;
using BDcource.Features.Workshops;
using BDcource.Helpers;
using BDcource.Models;
using System.Windows;

namespace BDcource
{
    public partial class WorkshopsWindow : Window
    {
        private readonly WorkshopService _workshopService;

        public WorkshopsWindow()
        {
            InitializeComponent();
            var context = new CourceContext();
            _workshopService = new WorkshopService(context);
            LoadWorkshops();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadWorkshops()
        {
            dgWorkshops.ItemsSource = _workshopService.GetAllWorkshops();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WorkshopEditDialog();
            if (dialog.ShowDialog() == true)
            {
                string error = _workshopService.AddWorkshop(dialog.Name, dialog.Address);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadWorkshops();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgWorkshops.SelectedItem as Workshop;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите цех", "Ошибка");
                return;
            }
            var dialog = new WorkshopEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                string error = _workshopService.UpdateWorkshop(selected.WorkshopId, dialog.Name, dialog.Address);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadWorkshops();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgWorkshops.SelectedItem as Workshop;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите цех", "Ошибка");
                return;
            }
            if (CustomMessageBox.Show($"Удалить цех \"{selected.WorkshopName}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _workshopService.DeleteWorkshop(selected.WorkshopId);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadWorkshops();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}