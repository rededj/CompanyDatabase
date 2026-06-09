using System.Linq;
using BDcource.Models;
using BDcource.Helpers;

namespace BDcource.Features.Auth
{
    public class AuthService
    {
        private readonly CourceContext _context;

        public AuthService(CourceContext context)
        {
            _context = context;
        }

        public User Authenticate(string login, string password)
        {
            string hash = PasswordHelper.ComputeSha256Hash(password);
            return _context.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == hash);
        }

        public bool IsFirstRun()
        {
            return !_context.Users.Any();
        }

        public void CreateFirstAdmin(string login, string password, string name, string position, string workshopName)
        {
            var adminRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Начальник");
            if (adminRole == null)
                throw new System.Exception("Роль 'Начальник' не найдена в БД");

            var user = new User
            {
                Login = login,
                PasswordHash = PasswordHelper.ComputeSha256Hash(password),
                RoleId = adminRole.RoleId,
                Name = name,
                Position = position,
                WorkshopName = workshopName
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}