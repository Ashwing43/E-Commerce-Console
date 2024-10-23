using Loader;
using Microsoft.Extensions.DependencyInjection;
using Models;
using ECommerce.Controllers;
using ECommerce.Models;
using ECommerce.Repositories;
using ECommerce.Services;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IProductRepository, ProductRepository>()
            .AddSingleton<IProductService, ProductService>()
            .AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IOrderRepository, OrderRepository>()
            .AddSingleton<IOrderService, OrderService>()
            .AddSingleton<UserController>()    
            .AddSingleton<ProductController>()
            .AddSingleton<OrderController>()
            .BuildServiceProvider();

        var userController = serviceProvider.GetService<UserController>();
        var productController = serviceProvider.GetService<ProductController>();
        var orderController = serviceProvider.GetService<OrderController>();


        while (true)
        {
            Console.WriteLine("Welcome to the E-Commerce App");
            Console.WriteLine("1. Sign Up");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            var choice = Console.ReadLine();
            if (choice == "1")
            {
                userController.SignUp();
            }
            else if (choice == "2")
            {
                var loggedInUser = userController.Login();
                
                if (loggedInUser == null) {
                    Console.WriteLine("\nUser not found, please sign up first.");
                    Thread.Sleep(1000);
                    Console.Clear();
                    continue;
                }

				if (loggedInUser.Name == "back")
				{
					Console.Clear();
					continue;
				}

				if (loggedInUser.Role == UserRole.Admin)
                {
                    AdminMenu(loggedInUser.Id, productController, userController, orderController);
                }
                else if (loggedInUser.Role == UserRole.Customer)
                {
                    Customer c = (Customer)loggedInUser;
                    if(c.Notifications.Count > 0)
                    {
                        int cnt = 0;
                        Console.WriteLine("\n****************New Notifications**********");
                        while (c.Notifications.Count > 0)
                        {
                            Console.WriteLine(c.Notifications.Dequeue());
                        }
                        Console.WriteLine();
                    }
                    CustomerMenu(loggedInUser.Id, productController, orderController, userController);
                }
            }
            else if (choice == "3")
            {
                Loader.Loader.RunExit();
                Console.Clear();
                break;
            }
            else if(choice == "back")
            {
                Console.WriteLine("Cannot go back from main menu. Please exit.");
                Thread.Sleep(1000);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Enter valid choice.");
                Thread.Sleep(500);
            }
            Console.Clear();
        }
    }

    public static void AdminMenu(Guid adminId, ProductController productController, UserController userController, OrderController orderController)
    {
        while (true)
        {
            //Console.Clear();
            Console.WriteLine("Admin Menu:");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. View All Products");
            Console.WriteLine("3. Show All Orders");
            Console.WriteLine("4. Update product details");
            Console.WriteLine("5. Change Order Status");
            Console.WriteLine("6. My Profile");
            Console.WriteLine("7. Edit Profile");
            Console.WriteLine("8. Logout");
            Console.WriteLine("Enter a choice:");

            bool breaker = false;

            var choice = Console.ReadLine();

            switch (choice) {
                case "1":
                    productController.AddProduct(adminId);
                    break;

                case "2":
                    productController.ShowAllProductsAdmin();
                    break;

                case "3":
                    orderController.ShowAllOrders();
                    break;

                case "4":
                    productController.UpdateProductDetails();
                    break;

                case "5":
                    orderController.ChangeOrderStatus();
                    break;

                case "6":
                    userController.DisplayInfo(adminId);
                    break;

                case "7":
                    userController.EditProfile(adminId);
                    break;

                case "8":
                    Loader.Loader.RunLogout();
                    Console.Clear();
                    breaker = true;
                    break;

                case "back":
                    Console.WriteLine("Cannot go back from admin menu. Please logout.");
                    Thread.Sleep(1000);
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Enter valid choice.");
                    Thread.Sleep(500);
                    Console.Clear();
                    break;
            }
            if (breaker) break;
        }
    }

    public static void CustomerMenu(Guid customerId, ProductController productController, OrderController orderController, UserController userController)
    {
        while (true)
        {
            Console.WriteLine("Customer Menu:");
            Console.WriteLine("1. View All Products");
            Console.WriteLine("2. Place Order");
            Console.WriteLine("3. View All Orders");
            Console.WriteLine("4. Cancel Order");
            Console.WriteLine("5. My Profile");
            Console.WriteLine("6. Edit Profile");
            Console.WriteLine("7. Check Order Status");
            Console.WriteLine("8. Logout");
            Console.WriteLine("Enter a choice:");

            bool breaker = false;

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    productController.ShowAllProductsCustomer();
                    break;

                case "2":
                    orderController.PlaceOrder(customerId);
                    break;

                case "3":
                    orderController.ShowAllOrders(customerId);
                    break;

                case "4":
                    orderController.CancelOrder(customerId);
                    break;

                case "5":
                    userController.DisplayInfo(customerId);
                    break;

                case "6":
                    userController.EditProfile(customerId);
                    break;

                case "7":
                    orderController.GetOrderStatus(customerId);
                    break;

                case "8":
                    Loader.Loader.RunLogout();
                    breaker = true;
                    Console.Clear();
                    break;

                case "back":
                    Console.WriteLine("Cannot go back from customer menu. Please logout.");
                    Thread.Sleep(1000);
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Enter valid choice.");
                    Thread.Sleep(500);
                    Console.Clear();
                    break;
            }
            if (breaker) break;
            
        }
    }
}