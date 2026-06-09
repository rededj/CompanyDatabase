using System.Windows;
using BDcource.Models;

namespace BDcource.Features.Reports
{
    public partial class Query6Window : Window
    {
        public Query6Window()
        {
            InitializeComponent();
            using (var context = new CourceContext())
            {
                var reportService = new ReportService(context);
                dgResult.ItemsSource = reportService.GetMaterialsSortedByUsage();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}