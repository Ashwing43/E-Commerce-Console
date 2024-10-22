using Models;
using Models.Contract;
using ECommerce.Models;
using ECommerce.Services;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using CustomExceptions;

namespace ECommerce.Controllers
{
    public class OrderController
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public OrderController(IOrderService orderService, IUserService userService, IProductService productService)
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;

            OrderService orderService1 = (OrderService)orderService;
            orderService1.OnOrderProcessed += HandleOrderProcessed;
        }

        public void PlaceOrder(Guid userId)
        {
            try
            {
                Customer user = (Customer)_userService.GetUserById(userId);

                if (user.Role != UserRole.Customer)
                {
                    Console.WriteLine("Only customers can place orders.");
                    return;
                }

                Console.WriteLine("Available products");
                List<Product> products = _productService.GetAllProducts().ToList();

                if (products.Count <= 0)
                {
                    Console.WriteLine("No products available");
                    return;
                }

                int i = 1;
                foreach (var p in products)
                {
                    Console.Write($"sr no. {i++},  ");
                    p.DisplayInfo();
                }
                Console.WriteLine();

                var productSr = new List<int>();
                List<Guid> productIds = new List<Guid>();
                decimal totalAmount = 0;

                while (true)
                {
                    Console.WriteLine("Enter a choice");
                    Console.WriteLine("1.Add product");
                    Console.WriteLine("2.Finish order");

                    var choice = "";
                    while (string.IsNullOrWhiteSpace(choice))
                    {
                        choice = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(choice))
                        {
                            Console.WriteLine("Input cannot be empty.");
                        }
                        else if (choice == "1" || choice == "2")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Enter valid choice.");
                        }
                    }

                    if (choice == "1")
                    {
                        Console.WriteLine("Enter Product sr. no. to order:");

                        var input = "";
                        int sr = 0;
                        while (true)
                        {
                            bool flag = false;
                            input = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(input))
                            {
                                Console.WriteLine("Input cannot be empty.");
                            }
                            else if (int.TryParse(input, out int a))
                            {
                                sr = a;
                                if (sr < 1 || sr > products.Count)
                                {
                                    Console.WriteLine($"Enter value between 1 and {products.Count}");
                                }
                                else
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Enter valid input");
                            }
                            if (flag) break;
                        }

                        var p = _productService.GetProductById(products[sr - 1].Id);
                        totalAmount += p.Price;
                        p.Quantity--;
                        productIds.Add(products[sr - 1].Id);
                        Console.WriteLine($"Added product with sr no. {sr}");
                        Console.WriteLine($"Total amount: {totalAmount}\n");
                    }
                    else if (choice == "2")
                    {
                        if (productIds.Count < 1)
                        {
                            Console.WriteLine("You have not selected anything.\nOrder cancelled.");
                        }
                        else
                        {
                            Console.WriteLine($"Your total amount for this order is {totalAmount}.");
                            var addresses = user.Addresses;

                            Address address;
                            int addressChoice = 0;
                            Console.WriteLine("\nChoose address");
                            int j = 0;
                            foreach (var add in addresses)
                            {
                                Console.WriteLine($"{j++}. {add.Street}, {add.City}, {add.ZipCode}");
                            }
                            Console.WriteLine($"{addresses.Count}. Enter new Address");

                            bool isInputCorrect;
                            do
                            {
                                isInputCorrect = int.TryParse(Console.ReadLine(), out int ch);
                                if (isInputCorrect) addressChoice = ch;

                                if (!(addressChoice <= addresses.Count && addressChoice >= 0))
                                {
                                    Console.WriteLine("Enter valid choice.");
                                    isInputCorrect = false;
                                }

                            } while (!isInputCorrect);

                            if (addressChoice == addresses.Count)
                            {
                                address = Utils.AddressReader.GetAddress();
                                addresses.Add(address);
                            }
                            else
                            {
                                address = addresses[addressChoice];
                            }

                            var order = new Order() { Id = Guid.NewGuid(), UserId = userId, ProductIds = productIds, TotalAmount = totalAmount, Address = address, OrderStatus = OrderStatus.Pending };

                            _orderService.PlaceOrderAsync(order);
                            Console.WriteLine("Order placed successfully.");
                        }

                        Console.WriteLine("\nPress any key to return to menu.");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public void ShowAllOrders()
        {
            try
            {
                var orders = _orderService.GetAllOrders().ToList();

                if (orders.Count <= 0)
                {
                    Console.WriteLine("No orders at the moment.");
                    Console.WriteLine("\nPress any key to return to menu.");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }

                foreach (var order in orders)
                {
                    Console.WriteLine($"Order ID: {order.Id}, User ID: {order.UserId}, Total Amount: {order.TotalAmount}");
                }
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

        public void ShowAllOrders(Guid UserId)
        {
            try
            {
                var orders = _orderService.GetAllOrdersByUserId(UserId);
                foreach (var order in orders)
                {
                    Console.WriteLine($"\nOrder ID: {order.Id}, Total Amount: {order.TotalAmount}, Status: {order.OrderStatus}");
                }
                Console.WriteLine("\nPress any key to return to menu");
                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public void GetOrderStatus(Guid UserId)
        {
            try
            {
                _orderService.GetOrderStatusAsync(UserId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public void ChangeOrderStatus()
        {
            try
            {
                _orderService.ChangeOrderStatus();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public void CancelOrder(Guid userId)
        {
            try
            {
                var orders = _orderService.GetAllOrdersByUserId(userId).Where(order => order.OrderStatus != OrderStatus.Delivered && order.OrderStatus != OrderStatus.Cancelled).ToList();
                if(orders.Count < 1)
                {
                    Console.WriteLine("No orders at the moment to cancel.");
                    Console.WriteLine("\nPress any key to return to menu");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                foreach (var order in orders)
                {
                    Console.WriteLine($"\nOrder ID: {order.Id}, Total Amount: {order.TotalAmount}, Status: {order.OrderStatus}");
                }

                Console.WriteLine("Enter order id to cancel:");
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
                    Order order = _orderService.GetOrderById(id);
                    if (order is null)
                        throw new OrderNotFoundException();

                    if (order.OrderStatus == OrderStatus.Cancelled)
                    {
                        Console.WriteLine("The order is already cancelled.");
                    }
                    else if (order.OrderStatus == OrderStatus.Delivered)
                    {
                        Console.WriteLine("The order is delivered. Cannot cancelled.");
                    }
                    else
                    {
                        order.OrderStatus = OrderStatus.Cancelled;
                        Console.WriteLine("Order has been cancelled");
                    }
                }
                catch (OrderNotFoundException ex)
                {
                    Console.WriteLine("Order cannot be found");
                    CustomLogger.Logger.LogError(ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong.");
                    CustomLogger.Logger.LogError(e);
                }

                Console.WriteLine("\nPress any key to return to menu");
                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        private void HandleOrderProcessed(Order o, string statused)
        {
            try
            {
                Console.WriteLine($"Order {statused}");
                Guid userId = o.UserId;
                Customer c = (Customer)_userService.GetUserById(userId);
                c.Notifications.Enqueue($"Your order with order id {o.Id} has been {statused}.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }
    }
}
