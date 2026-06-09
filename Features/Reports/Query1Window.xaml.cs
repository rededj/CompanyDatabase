using System;
using System.Linq;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.Reports
{
    public partial class Query1Window : Window
    {
        private readonly CourceContext _context;
        private readonly ReportService _reportService;

        public Query1Window()
        {
            InitializeComponent();
            _context = new CourceContext();
            _reportService = new ReportService(_context);
            LoadWorkshops();
        }

        private void LoadWorkshops()
        {
            cbWorkshop.ItemsSource = _context.Workshops.ToList();
        }

        private void ShowReport_Click(object sender, RoutedEventArgs e)
        {
            if (cbWorkshop.SelectedValue == null)
            {
                CustomMessageBox.Show("Выберите цех");
                return;
            }
            int workshopId = (int)cbWorkshop.SelectedValue;
            var data = _reportService.GetOperationsWithMaterials(workshopId);
            dgResult.ItemsSource = data;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}