using ECommerce.Models;
using static ECommerce.Services.OrderService;

namespace ECommerce.Services
{
    public interface IOrderService
    {
        void PlaceOrderAsync(Order order);
        Order GetOrderById(Guid id);
        IEnumerable<Order> GetAllOrders();

        IEnumerable<Order> GetAllOrdersByUserId(Guid UserId);

        void GetOrderStatusAsync(Guid userId);

        public void ChangeOrderStatus();

        public delegate void OrderProcessedEventHandler();
    }
}
