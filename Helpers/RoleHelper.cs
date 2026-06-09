using BDcource.Models;

namespace BDcource.Helpers
{
    public static class RoleHelper
    {
        public static bool IsAdmin(User user) => user?.Role?.RoleName == "Начальник";
        public static bool IsEmployee(User user) => user?.Role?.RoleName == "Сотрудник";
    }
}