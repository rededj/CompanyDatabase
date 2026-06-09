using System.Windows;
using BDcource.Models;

namespace BDcource.Features.Reports
{
    public partial class Query9Window : Window
    {
        public Query9Window()
        {
            InitializeComponent();
            using (var context = new CourceContext())
            {
                var reportService = new ReportService(context);
                dgResult.ItemsSource = reportService.GetProductionReport();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}