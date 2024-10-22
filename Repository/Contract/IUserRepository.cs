using ECommerce.Models;
using Service.Contract;

namespace ECommerce.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        void Add(User user);
        User GetById(Guid id);
        IEnumerable<User> GetAll();
    }
}
