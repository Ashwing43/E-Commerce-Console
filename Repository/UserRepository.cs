using DataStorage;
using ECommerce.Models;

namespace ECommerce.Repositories
{
    public class UserRepository : IUserRepository
    {
        public void Add(User user)
        {
            DataStore.Users.Add(user);
        }

        public User GetById(Guid id)
        {
            return DataStore.Users.FirstOrDefault(u => u.Id == id);
        }

        public IEnumerable<User> GetAll()
        {
            return DataStore.Users;
        }
    }
}
