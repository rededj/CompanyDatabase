using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.Blueprints
{
    public partial class BlueprintEditDialog : Window
    {
        public string BlueprintNumber { get; private set; }
        public string TechnicalRequirements { get; private set; }

        private readonly Blueprint _editBlueprint;

        public BlueprintEditDialog(Blueprint editBlueprint = null)
        {
            InitializeComponent();
            _editBlueprint = editBlueprint;
            if (editBlueprint != null)
            {
                txtBlueprintNumber.Text = editBlueprint.BlueprintNumber;
                txtBlueprintNumber.IsReadOnly = true; 
                txtTechnicalRequirements.Text = editBlueprint.TechnicalRequirements;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBlueprintNumber.Text))
            {
                CustomMessageBox.Show("Введите номер чертежа", "Ошибка");
                return;
            }
            BlueprintNumber = txtBlueprintNumber.Text.Trim();
            TechnicalRequirements = txtTechnicalRequirements.Text.Trim();
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