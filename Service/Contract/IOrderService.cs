using Delegates;
using ECommerce.Models;

namespace ECommerce.Services
{
    public interface IOrderService
    {
        void PlaceOrderAsync(Order order);
        Order GetOrderById(Guid id);
        IEnumerable<Order> GetAllOrders();

        IEnumerable<Order> GetAllOrdersByUserId(Guid UserId);

        public void ChangeOrderStatus();

		public OrderProcessedEventHandler OnOrderProcessed { set; get; }
	}
}
