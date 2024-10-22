using Models;
using ECommerce.Models;
using System.Globalization;

namespace Factories
{
    public static class UserFactory
    {
        public static User CreateUser(string name, string email, string password, UserRole userRole)
        {
            if(userRole == UserRole.Admin)
            {
                return new Admin()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Email = email,
                    Role = userRole,
                    Password = password
                };
            }
            return new Customer()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Email = email,
                    Role = userRole,
                    Password = password
                };
        }
    }
}
