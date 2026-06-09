using System;
using System.Windows;
using BDcource.Helpers;
using BDcource.Models;

namespace BDcource.Features.Reports
{
    public partial class Query4Window : Window
    {
        public Query4Window()
        {
            InitializeComponent();
            dpStart.SelectedDate = DateTime.Now.AddMonths(-1);
            dpEnd.SelectedDate = DateTime.Now;
        }

        private void ShowReport_Click(object sender, RoutedEventArgs e)
        {
            if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                CustomMessageBox.Show("Выберите даты");
                return;
            }
            DateOnly start = DateOnly.FromDateTime(dpStart.SelectedDate.Value);
            DateOnly end = DateOnly.FromDateTime(dpEnd.SelectedDate.Value);
            using (var context = new CourceContext())
            {
                var reportService = new ReportService(context);
                var data = reportService.GetWorkOrdersWithTools(start, end);
                dgResult.ItemsSource = data;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}