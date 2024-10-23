using CustomLogger;
using CustomExceptions;
using Microsoft.Extensions.Logging;
using ECommerce.Models;
using ECommerce.Repositories;
using System.Xml.Serialization;

namespace ECommerce.Services
{
    public delegate void OrderProcessedEventHandler(Order o, string status);
    public class OrderService : IOrderService
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public void PlaceOrderAsync(Order order)
        {
            try
            {
                Task.Run(() => _orderRepository.Add(order));
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public Order GetOrderById(Guid id)
        {
            try
            {
                return _orderRepository.GetById(id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
                return null;
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            try
            {
                return _orderRepository.GetAll();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
                throw;
            }
        }

        public IEnumerable<Order> GetAllOrdersByUserId(Guid UserId)
        {
            try
            {
                var orders = _orderRepository.GetAll().Where(order => order.UserId == UserId);
                return orders;
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
                return new Order[0];
            }
        }

        public void GetOrderStatusAsync(Guid userId)
        {
            try
            {
                var orders = GetAllOrdersByUserId(userId);
                foreach (Order o in orders)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Order Id: {o.Id},  Amount:{o.TotalAmount},  Order Status: {o.OrderStatus}");
                    Console.WriteLine($"Products:");
                    foreach(Guid prodId in o.ProductIds)
                    {
                        Product p = _productRepository.GetById(prodId);
                        Console.WriteLine($"Name: {p.Name}, Price: {p.Price}");
                    }
                }
                Console.WriteLine("\nEnter order Id to check status");
                var input = "";
                Guid id = new Guid();
                while (string.IsNullOrWhiteSpace(input))
                {
                    input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Input cannot be empty.");
                    }
                    else if (Guid.TryParse(input, out Guid k))
                    {
                        id = k;
                        break;
                    }
                }

                Order order = _orderRepository.GetById(id);
                try
                {
                    if (order == null)
                    {
                        throw new OrderNotFoundException();
                    }
                    Console.WriteLine($"Order status is {order.OrderStatus}");

                }
                catch (OrderNotFoundException ex)
                {
                    Console.WriteLine("Order not found.");
                    CustomLogger.Logger.LogError(ex);
                }
                finally
                {
                    Console.WriteLine("\nPress any key to return to menu.");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public void ChangeOrderStatus()
        {
            var orders = _orderRepository.GetAll().Where(o => o.OrderStatus != OrderStatus.Cancelled && o.OrderStatus != OrderStatus.Delivered).ToList();

            if (orders.Count < 1)
            {
                Console.WriteLine("No orders at the moment.");
                Console.WriteLine("\nPress any key to return to menu.");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            foreach (Order o in orders)
            {
                Console.WriteLine($"Order Id: {o.Id},  User Id: {o.UserId},  Amount:{o.TotalAmount},  Order Status: {o.OrderStatus}");
            }
            Console.WriteLine("Enter order id");
            var input = "";
            Guid id = new Guid();
            while (string.IsNullOrWhiteSpace(input))
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty.");
                }
                else if (Guid.TryParse(input, out Guid k))
                {
                    id = k;
                    break;
                }
            }

            try
            {
                Order order = _orderRepository.GetById(id);
                if (order is null)
                    throw new OrderNotFoundException();

                Console.WriteLine("Change status to?");
                Console.WriteLine("1. Processed");
                Console.WriteLine("2. Shipped");
                Console.WriteLine("3. Delivered");
                OrderStatus newStatus = OrderStatus.Pending;
                bool isStatusChanged = false;
                input = "";
                while (true)
                {
                    input = Console.ReadLine();
                    if (input == "1" || input == "2" || input == "3" || input == "4") break;
                    else
                    {
                        Console.WriteLine("Enter valid choice");
                    }
                }

                switch (input)
                {
                    case "1":
                        newStatus = OrderStatus.Processed;
                        break;

                    case "2":
                        newStatus = OrderStatus.Shipped;
                        break;

                    case "3":
                        newStatus = OrderStatus.Delivered;
                        break;
                }

                string status = "";
                switch (input)
                {
                    case "1":
                        if (order.OrderStatus == OrderStatus.Pending)
                        {
                            order.OrderStatus = OrderStatus.Processed;
                            status = "processed";
                            isStatusChanged = true;
                        }
                        else
                        {
                            Console.WriteLine($"Cannot change the status. The order is already {order.OrderStatus}.");
                        }
                        break;

                    case "2":
                        if (order.OrderStatus == OrderStatus.Processed)
                        {
                            order.OrderStatus = OrderStatus.Shipped;
                            status = "shipped";
                            isStatusChanged = true;
                        }
                        else if (order.OrderStatus == OrderStatus.Pending)
                        {
                            Console.WriteLine($"Cannot change status to {newStatus}, It needs to be processed first.");
                        }
                        else
                        {
                            Console.WriteLine($"Cannot change the status to {newStatus}. The order is already {order.OrderStatus}.");
                        }
                        break;

                    case "3":
                        if (order.OrderStatus == OrderStatus.Shipped)
                        {
                            order.OrderStatus = OrderStatus.Delivered;
                            status = "delivered";
                            isStatusChanged = true;
                        }
                        else if (order.OrderStatus == OrderStatus.Pending || order.OrderStatus == OrderStatus.Processed)
                        {
                            Console.WriteLine($"Cannot change status to {newStatus}, It needs to be shipped first.");
                        }
                        else
                        {
                            Console.WriteLine($"Cannot change the status. The order is already {order.OrderStatus}.");
                        }
                        break;
                }

                if (isStatusChanged)
                {
                    OnOrderProcessed.Invoke(order, status);
                }
                Console.WriteLine("\nPress any key to return to menu.");
                Console.ReadKey();
                Console.Clear();
            }
            catch (OrderNotFoundException ex)
            {
                Console.WriteLine("Order cannot be found");
                Logger.LogError(ex);

                Console.WriteLine("\nPress any key to return to menu.");
                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public OrderProcessedEventHandler OnOrderProcessed;

        public event OrderProcessedEventHandler myEvent
        {
            add { OnOrderProcessed += value; }
            remove { OnOrderProcessed -= value; }
        }

        public void RaiseEvent(Order o, string status)
        {
            if (OnOrderProcessed != null)
            {
                this.OnOrderProcessed(o, status);
            }
        }

    }
}
