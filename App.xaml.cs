using System.Linq;
using System.Windows;
using BDcource.Models;
using BDcource.Features.Auth;
using BDcource.Helpers;
using Microsoft.Extensions.Configuration;

namespace BDcource
{
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=cource;Trusted_Connection=True;TrustServerCertificate=True;";
            DatabaseInitializer.EnsureDatabaseCreated(connectionString);
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}