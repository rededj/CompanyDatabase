using System.Windows;
using BDcource.Models;

namespace BDcource.Features.Reports
{
    public partial class Query7Window : Window
    {
        public Query7Window()
        {
            InitializeComponent();
            using (var context = new CourceContext())
            {
                var reportService = new ReportService(context);
                dgResult.ItemsSource = reportService.GetProductsWithTools();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}