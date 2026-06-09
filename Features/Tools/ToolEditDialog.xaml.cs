using System;
using System.Linq;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.Tools
{
    public partial class ToolEditDialog : Window
    {
        public string SerialNumber { get; private set; }
        public int ToolTypeId { get; private set; }
        public DateOnly ArrivalDate { get; private set; }

        private readonly Tool _editTool;
        private readonly CourceContext _context;

        public ToolEditDialog(Tool editTool = null)
        {
            InitializeComponent();
            _context = new CourceContext();
            _editTool = editTool;

            cbToolType.ItemsSource = _context.ToolTypes.ToList();

            if (editTool != null)
            {
                txtSerialNumber.Text = editTool.SerialNumber;
                txtSerialNumber.IsReadOnly = true; 
                cbToolType.SelectedValue = editTool.ToolTypeId;
                dpArrivalDate.SelectedDate = editTool.ArrivalDate.ToDateTime(TimeOnly.MinValue);
                Header.Text = "Редактирование инструмента";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSerialNumber.Text))
            {
                CustomMessageBox.Show("Введите серийный номер", "Ошибка");
                return;
            }
            if (cbToolType.SelectedValue == null)
            {
                CustomMessageBox.Show("Выберите тип инструмента", "Ошибка");
                return;
            }
            if (dpArrivalDate.SelectedDate == null)
            {
                CustomMessageBox.Show("Выберите дату поступления", "Ошибка");
                return;
            }

            SerialNumber = txtSerialNumber.Text.Trim();
            ToolTypeId = (int)cbToolType.SelectedValue;
            ArrivalDate = DateOnly.FromDateTime(dpArrivalDate.SelectedDate.Value);

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