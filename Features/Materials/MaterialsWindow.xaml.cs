using System.Windows;
using BDcource.Models;
using BDcource.Helpers;
using BDcource.Features.Materials;

namespace BDcource
{
    public partial class MaterialsWindow : Window
    {
        private readonly MaterialService _materialService;

        public MaterialsWindow()
        {
            InitializeComponent();
            var context = new CourceContext();
            _materialService = new MaterialService(context);
            LoadMaterials();

            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadMaterials()
        {
            dgMaterials.ItemsSource = _materialService.GetAllMaterials();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MaterialEditDialog();
            if (dialog.ShowDialog() == true)
            {
                _materialService.AddMaterial(dialog.Name, dialog.NumberOf);
                LoadMaterials();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgMaterials.SelectedItem as Material;
            if (selected == null)
            {
                CustomMessageBox.Show("Выберите материал", "Ошибка");
                return;
            }
            var dialog = new MaterialEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                _materialService.UpdateMaterial(selected.MaterialId, dialog.Name, dialog.NumberOf);
                LoadMaterials();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgMaterials.SelectedItem as Models.Material;
            if (selected == null) { CustomMessageBox.Show("Выберите материал"); return; }
            string error = _materialService.DeleteMaterial(selected.MaterialId);
            if (error != null)
                CustomMessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                LoadMaterials();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}