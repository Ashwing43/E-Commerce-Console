using ECommerce.Models;
using Service.Contract;

namespace ECommerce.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Add(Order order);
        Order GetById(Guid id);
        IEnumerable<Order> GetAll();
    }
}
