using System;
using System.Linq;
using System.Windows;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.Operations
{
    public partial class OperationEditDialog : Window
    {
        public string Description { get; private set; }
        public TimeOnly AverageDuration { get; private set; }
        public int WorkshopId { get; private set; }
        public string BlueprintNumber { get; private set; }

        private readonly Operation _editOperation;
        private readonly CourceContext _context;
        private readonly bool _isAdmin;
        private readonly bool _readOnly;
        private OperationMaterialNormService _materialNormService;
        private OperationToolNormService _toolNormService;

        public OperationEditDialog() : this(null, false) { }

        public OperationEditDialog(Operation editOperation, bool readOnly = false)
        {
            InitializeComponent();
            _context = new CourceContext();
            _editOperation = editOperation;
            _isAdmin = RoleHelper.IsAdmin(App.CurrentUser);
            _readOnly = readOnly || !_isAdmin; 

            cbWorkshop.ItemsSource = _context.Workshops.ToList();
            cbBlueprint.ItemsSource = _context.Blueprints.ToList();

            if (_readOnly)
            {
                txtDescription.IsReadOnly = true;
                txtAverageDuration.IsReadOnly = true;
                cbWorkshop.IsEnabled = false;
                cbBlueprint.IsEnabled = false;
                btnOk.Visibility = Visibility.Collapsed; 
                btnAddMaterial.Visibility = Visibility.Collapsed;
                btnEditMaterial.Visibility = Visibility.Collapsed;
                btnDeleteMaterial.Visibility = Visibility.Collapsed;
                btnAddTool.Visibility = Visibility.Collapsed;
                btnEditTool.Visibility = Visibility.Collapsed;
                btnDeleteTool.Visibility = Visibility.Collapsed;
            }
            else if (!_isAdmin)
            {
                btnAddMaterial.Visibility = Visibility.Collapsed;
                btnEditMaterial.Visibility = Visibility.Collapsed;
                btnDeleteMaterial.Visibility = Visibility.Collapsed;
                btnAddTool.Visibility = Visibility.Collapsed;
                btnEditTool.Visibility = Visibility.Collapsed;
                btnDeleteTool.Visibility = Visibility.Collapsed;
            }

            if (editOperation != null)
            {
                txtDescription.Text = editOperation.Description;
                txtAverageDuration.Text = editOperation.AverageDuration.ToString("HH:mm");
                cbWorkshop.SelectedValue = editOperation.WorkshopId;
                cbBlueprint.SelectedValue = editOperation.BlueprintNumber;
                LoadMaterialNorms(editOperation.OperationId);
                LoadToolNorms(editOperation.OperationId);
            }
        }

        private void LoadMaterialNorms(int operationId)
        {
            _materialNormService = new OperationMaterialNormService(_context);
            var norms = _materialNormService.GetMaterialsForOperation(operationId);
            dgMaterials.ItemsSource = norms;
        }

        private void LoadToolNorms(int operationId)
        {
            _toolNormService = new OperationToolNormService(_context);
            var norms = _toolNormService.GetToolsForOperation(operationId);
            dgTools.ItemsSource = norms;
        }

        private void BtnAddMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (_editOperation == null) { CustomMessageBox.Show("Сначала сохраните операцию."); return; }
            var availableMaterials = _context.Materials
                .Where(m => !_context.OperationMaterialsUsages.Any(omu => omu.OperationId == _editOperation.OperationId && omu.MaterialId == m.MaterialId))
                .ToList();
            if (!availableMaterials.Any())
            {
                CustomMessageBox.Show("Нет доступных материалов для добавления.");
                return;
            }
            var dialog = new MaterialNormEditDialog(availableMaterials);
            if (dialog.ShowDialog() == true)
            {
                string error = _materialNormService.AddMaterial(_editOperation.OperationId, dialog.MaterialId, dialog.RequiredQuantity);
                if (error != null) CustomMessageBox.Show(error);
                else LoadMaterialNorms(_editOperation.OperationId);
            }
        }

        private void BtnEditMaterial_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgMaterials.SelectedItem as OperationMaterialsUsage;
            if (selected == null) { CustomMessageBox.Show("Выберите материал."); return; }
            var dialog = new MaterialNormEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                string error = _materialNormService.UpdateMaterial(_editOperation.OperationId, selected.MaterialId, dialog.RequiredQuantity);
                if (error != null) CustomMessageBox.Show(error);
                else LoadMaterialNorms(_editOperation.OperationId);
            }
        }

        private void BtnDeleteMaterial_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgMaterials.SelectedItem as OperationMaterialsUsage;
            if (selected == null) { CustomMessageBox.Show("Выберите материал."); return; }
            if (CustomMessageBox.Show($"Удалить материал '{selected.Material.Name}' из норматива?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _materialNormService.DeleteMaterial(_editOperation.OperationId, selected.MaterialId);
                if (error != null) CustomMessageBox.Show(error);
                else LoadMaterialNorms(_editOperation.OperationId);
            }
        }

        private void BtnAddTool_Click(object sender, RoutedEventArgs e)
        {
            if (_editOperation == null) { CustomMessageBox.Show("Сначала сохраните операцию."); return; }
            var availableToolTypes = _context.ToolTypes
                .Where(tt => !_context.OperationToolsUsages.Any(otu => otu.OperationId == _editOperation.OperationId && otu.ToolTypeId == tt.ToolTypeId))
                .ToList();
            if (!availableToolTypes.Any())
            {
                CustomMessageBox.Show("Нет доступных типов инструментов для добавления.");
                return;
            }
            var dialog = new ToolNormEditDialog(availableToolTypes);
            if (dialog.ShowDialog() == true)
            {
                string error = _toolNormService.AddTool(_editOperation.OperationId, dialog.ToolTypeId, dialog.QuantityInUse);
                if (error != null) CustomMessageBox.Show(error);
                else LoadToolNorms(_editOperation.OperationId);
            }
        }

        private void BtnEditTool_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgTools.SelectedItem as OperationToolsUsage;
            if (selected == null) { CustomMessageBox.Show("Выберите тип инструмента."); return; }
            var dialog = new ToolNormEditDialog(selected);
            if (dialog.ShowDialog() == true)
            {
                string error = _toolNormService.UpdateTool(_editOperation.OperationId, selected.ToolTypeId, dialog.QuantityInUse);
                if (error != null) CustomMessageBox.Show(error);
                else LoadToolNorms(_editOperation.OperationId);
            }
        }

        private void BtnDeleteTool_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgTools.SelectedItem as OperationToolsUsage;
            if (selected == null) { CustomMessageBox.Show("Выберите тип инструмента."); return; }
            if (CustomMessageBox.Show($"Удалить инструмент '{selected.ToolType.Name}' из норматива?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string error = _toolNormService.DeleteTool(_editOperation.OperationId, selected.ToolTypeId);
                if (error != null) CustomMessageBox.Show(error);
                else LoadToolNorms(_editOperation.OperationId);
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_readOnly) return;
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            { CustomMessageBox.Show("Введите описание"); return; }
            if (!TimeOnly.TryParseExact(txtAverageDuration.Text, "HH:mm", null, System.Globalization.DateTimeStyles.None, out TimeOnly duration))
            {
                CustomMessageBox.Show("Введите корректную длительность в формате ЧЧ:ММ");
                return;
            }
            if (cbWorkshop.SelectedValue == null) { CustomMessageBox.Show("Выберите цех"); return; }
            if (cbBlueprint.SelectedValue == null) { CustomMessageBox.Show("Выберите чертёж"); return; }

            Description = txtDescription.Text.Trim();
            AverageDuration = duration;
            WorkshopId = (int)cbWorkshop.SelectedValue;
            BlueprintNumber = cbBlueprint.SelectedValue.ToString();

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