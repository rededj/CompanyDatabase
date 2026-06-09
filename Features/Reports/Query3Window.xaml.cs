using System.Windows;
using BDcource.Models;

namespace BDcource.Features.Reports
{
    public partial class Query3Window : Window
    {
        public Query3Window()
        {
            InitializeComponent();
            using (var context = new CourceContext())
            {
                var reportService = new ReportService(context);
                dgResult.ItemsSource = reportService.GetToolsSortedByUsage();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}