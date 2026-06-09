using BDcource.Helpers;
using BDcource.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BDcource.Features.Users
{
    public class UserService
    {
        private readonly CourceContext _context;

        public UserService(CourceContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.Include(u => u.Role).ToList();
        }

        public void AddUser(string login, string password, int roleId, string name, string position, string workshopName)
        {
            var hash = PasswordHelper.ComputeSha256Hash(password);
            var user = new User
            {
                Login = login,
                PasswordHash = hash,
                RoleId = roleId,
                Name = name,
                Position = position,
                WorkshopName = workshopName
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(int userId, int newRoleId, string newPassword, string name, string position, string workshopName)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return;
            user.RoleId = newRoleId;
            if (!string.IsNullOrEmpty(newPassword))
                user.PasswordHash = PasswordHelper.ComputeSha256Hash(newPassword);
            user.Name = name;
            user.Position = position;
            user.WorkshopName = workshopName;
            _context.SaveChanges();
        }

        public string DeleteUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return "Пользователь не найден";

            bool hasMaterialIssuances = _context.MaterialIssuances.Any(mi => mi.UserId == userId);
            bool hasToolIssuances = _context.ToolIssuances.Any(ti => ti.UserId == userId);
            if (hasMaterialIssuances || hasToolIssuances)
                return "Нельзя удалить пользователя, так как он участвовал в выдаче материалов или инструментов.";

            _context.Users.Remove(user);
            _context.SaveChanges();
            return null;
        }

        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }
    }
}