using System.Windows;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.Blueprints
{
    public partial class BlueprintsWindow : Window
    {
        private readonly BlueprintService _blueprintService;

        public BlueprintsWindow()
        {
            InitializeComponent();
            var context = new CourceContext();
            _blueprintService = new BlueprintService(context);
            LoadBlueprints();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadBlueprints()
        {
            dgBlueprints.ItemsSource = _blueprintService.GetAllBlueprints();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new BlueprintEditDialog();
            if (dialog.ShowDialog() == true)
            {
                _blueprintService.AddBlueprint(dialog.BlueprintNumber, dialog.TechnicalRequirements);
                LoadBlueprints();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBlueprints.SelectedItem as Blueprint;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите чертеж", "Ошибка");
                return;
            }
            var dialog = new BlueprintEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                _blueprintService.UpdateBlueprint(selected.BlueprintNumber, dialog.TechnicalRequirements);
                LoadBlueprints();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBlueprints.SelectedItem as Blueprint;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите чертёж", "Ошибка");
                return;
            }
            if (CustomMessageBox.Show($"Удалить чертёж {selected.BlueprintNumber}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _blueprintService.DeleteBlueprint(selected.BlueprintNumber);
                if (error != null)
                    CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    LoadBlueprints();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}