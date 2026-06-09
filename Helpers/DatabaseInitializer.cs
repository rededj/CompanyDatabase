using Microsoft.Data.SqlClient;
using System.IO;
using System.Text;

namespace BDcource.Helpers
{
    public static class DatabaseInitializer
    {
        public static void EnsureDatabaseCreated(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            string dbName = builder.InitialCatalog;
            string masterConnection = connectionString.Replace($"Database={dbName}", "Database=master");

            // 1. Создаём базу данных, если её нет
            using (var conn = new SqlConnection(masterConnection))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{dbName}'";
                int dbCount = (int)cmd.ExecuteScalar();
                if (dbCount == 0)
                {
                    cmd.CommandText = $"CREATE DATABASE [{dbName}] COLLATE Cyrillic_General_CI_AS";
                    cmd.ExecuteNonQuery();
                }
            }

            // 2. Выполняем SQL-скрипт (создание таблиц, ключей, триггеров)
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Проверяем наличие таблицы Products (признак того, что скрипт уже выполнялся)
                var cmdCheck = conn.CreateCommand();
                cmdCheck.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'Products'";
                int tablesExist = (int)cmdCheck.ExecuteScalar();
                if (tablesExist == 0)
                {
                    string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "c.sql");
                    if (!File.Exists(scriptPath))
                        throw new FileNotFoundException("SQL script not found: c.sql");
                    string script = File.ReadAllText(scriptPath, Encoding.UTF8);
                    var commands = script.Split(new[] { "GO\r\n", "GO\n", "GO" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var cmdText in commands)
                    {
                        if (string.IsNullOrWhiteSpace(cmdText)) continue;
                        using (var sqlCmd = conn.CreateCommand())
                        {
                            sqlCmd.CommandText = cmdText;
                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                }

                var cmdRoleChief = conn.CreateCommand();
                cmdRoleChief.CommandText = "IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Начальник') INSERT INTO Roles (RoleName) VALUES ('Начальник')";
                cmdRoleChief.ExecuteNonQuery();

                var cmdRoleEmployee = conn.CreateCommand();
                cmdRoleEmployee.CommandText = "IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Сотрудник') INSERT INTO Roles (RoleName) VALUES ('Сотрудник')";
                cmdRoleEmployee.ExecuteNonQuery();

                var cmdUser = conn.CreateCommand();
                cmdUser.CommandText = @"
                    IF NOT EXISTS (SELECT 1 FROM Users WHERE Login = 'admin1')
                    INSERT INTO Users (Login, PasswordHash, RoleID, Name, Position, WorkshopName)
                    VALUES ('admin1', '5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5', 
                            (SELECT RoleID FROM Roles WHERE RoleName = 'Начальник'), 
                            'Полуэктов Максим Александрович', 'Старший мастер', NULL)";
                cmdUser.ExecuteNonQuery();
            }
        }
    }
}