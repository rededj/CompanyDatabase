using BDcource.Features.Blueprints;
using BDcource.Features.MaterialIssuanceForms;
using BDcource.Features.Operations;
using BDcource.Features.Reports;
using BDcource.Features.ToolIssuanceForms;
using BDcource.Features.Tools;
using BDcource.Features.ToolTypes;
using BDcource.Features.Users;
using BDcource.Features.WorkOrders;
using BDcource.Helpers;
using BDcource.Models;
using System;
using System.Windows;

namespace BDcource
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            var monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            for (int i = 0; i < 12; i++)
            {
                cbMonth.Items.Add(monthNames[i]);
            }
            cbMonth.SelectedIndex = DateTime.Now.Month - 1; 

            int year = DateTime.Now.Year;
            for (int i = year - 2; i <= year; i++) cbYear.Items.Add(i);
            cbYear.SelectedItem = DateTime.Now.Year;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserLogin.Text = App.CurrentUser.Login;
            txtUserRole.Text = App.CurrentUser.Role?.RoleName ?? "Не определена";
            if (RoleHelper.IsEmployee(App.CurrentUser))
            {
                tabDirectories.Visibility = Visibility.Collapsed;
                mainTabControl.SelectedIndex = 1;
            }
            else 
            {
                mainTabControl.SelectedIndex = 0;
            }
            if (RoleHelper.IsAdmin(App.CurrentUser))
            {
                tabUsers.Visibility = Visibility.Visible;
            }
        }

        private void MaterialIssuanceControl_Loaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Features.MaterialIssuanceForms.MaterialIssuanceControl;
            control?.RefreshData();
        }

        private void ToolIssuanceControl_Loaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Features.ToolIssuanceForms.ToolIssuanceControl;
            control?.RefreshData();
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            var win = new ProductsWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnWorkshops_Click(object sender, RoutedEventArgs e)
        {
            var win = new WorkshopsWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnOperations_Click(object sender, RoutedEventArgs e)
        {
            var win = new OperationsWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnMaterials_Click(object sender, RoutedEventArgs e)
        {
            var win = new MaterialsWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnToolTypes_Click(object sender, RoutedEventArgs e)
        {
            var win = new ToolTypesWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnTools_Click(object sender, RoutedEventArgs e)
        {
            var win = new ToolsWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnWorkOrders_Click(object sender, RoutedEventArgs e)
        {
            var win = new WorkOrdersWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnBlueprints_Click(object sender, RoutedEventArgs e)
        {
            var win = new BlueprintsWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query1_Click(object sender, RoutedEventArgs e)
        {
            var win = new Query1Window();
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query2_Click(object sender, RoutedEventArgs e)
        {
            var win = new ToolsWindow(); 
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query3_Click(object sender, RoutedEventArgs e)
        {
            var win = new Query3Window();
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query4_Click(object sender, RoutedEventArgs e)
        {
            var win = new Query4Window();
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query5_Click(object sender, RoutedEventArgs e)
        {
            var win = new OperationsWindow(); 
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query6_Click(object sender, RoutedEventArgs e)
        {
            var win = new Query6Window();
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query7_Click(object sender, RoutedEventArgs e)
        {
            var win = new Query7Window();
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query8_Click(object sender, RoutedEventArgs e)
        {
            var win = new WorkOrdersWindow(); 
            win.Owner = this;
            win.ShowDialog();
        }

        private void Query9_Click(object sender, RoutedEventArgs e)
        {
            var win = new Query9Window();
            win.Owner = this;
            win.ShowDialog();
        }

        private void GenerateMonthlyReport_Click(object sender, RoutedEventArgs e)
        {
            if (cbMonth.SelectedIndex == -1 || cbYear.SelectedItem == null)
            {
                CustomMessageBox.Show("Выберите месяц и год");
                return;
            }

            int month = cbMonth.SelectedIndex + 1; 
            int year = (int)cbYear.SelectedItem;

            using (var context = new CourceContext())
            {
                var reportService = new MonthlyReportService(context);
                var report = reportService.GetMonthlyReport(month, year);
                dgMonthlyReport.ItemsSource = report;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}