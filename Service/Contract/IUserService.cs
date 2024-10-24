using ECommerce.Models;

namespace ECommerce.Services
{
    public interface IUserService
    {
        void AddUser(User user);
        User GetUserById(Guid id);
        IEnumerable<User> GetAllUsers();
        void DisplayInfo(Guid id);
    }
}
