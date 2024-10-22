using DataStorage;
using ECommerce.Models;

namespace ECommerce.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public void Add(Order order)
        {
            DataStore.Orders.Add(order);
        }
        public Order GetById(Guid id)
        {
            return DataStore.Orders.FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Order> GetAll()
        {
            return DataStore.Orders;
        }
    }
}
