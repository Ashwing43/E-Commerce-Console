using Loader;
using Microsoft.Extensions.DependencyInjection;
using Models;
using ECommerce.Controllers;
using ECommerce.Models;
using ECommerce.Repositories;
using ECommerce.Services;
using System.Globalization;
using Constants;

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
			Console.Clear();
			Console.WriteLine("Welcome to the E-Commerce App");
			Console.WriteLine("1. Sign Up");
			Console.WriteLine("2. Login");
			Console.WriteLine("3. Exit");

			var choice = Console.ReadLine();

			if (choice == Choice_Constants.ONE)
			{
				userController.SignUp();
				continue;
			}

			if (choice == Choice_Constants.TWO)
			{
				var loggedInUser = userController.Login();

				if (loggedInUser == null)
				{
					Console.WriteLine("\nUser not found, please sign up first.");
					Thread.Sleep(1000);
					continue;
				}

				if (loggedInUser.Name == Constants.Constants.BACK)
				{
					continue;
				}

				if (loggedInUser.Role == UserRole.Admin)
				{
					AdminMenu(loggedInUser.Id, productController, userController, orderController);
					continue;
				}

				if (loggedInUser.Role == UserRole.Customer)
				{
					var customer = (Customer)loggedInUser;

					if (customer.Notifications.Count > 0)
					{
						Console.WriteLine("\n**************** New Notifications ****************");
						while (customer.Notifications.Count > 0)
						{
							Console.WriteLine(customer.Notifications.Dequeue());
						}
						Console.WriteLine();
					}

					CustomerMenu(loggedInUser.Id, productController, orderController, userController);
					continue;
				}
			}

			if (choice == Choice_Constants.THREE)
			{
				Loader.Loader.RunExit();
				break;
			}

			if (choice == Constants.Constants.BACK)
			{
				Console.WriteLine("Cannot go back from main menu. Please exit.");
				Thread.Sleep(1200);
				continue;
			}

			Console.WriteLine("Enter valid choice.");
			Thread.Sleep(1000);
		}
	}

	public static void AdminMenu(Guid adminId, ProductController productController, UserController userController, OrderController orderController)
	{
		while (true)
		{
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

			var choice = Console.ReadLine();

			switch (choice)
			{
				case Choice_Constants.ONE:
					productController.AddProduct(adminId);
					break;

				case Choice_Constants.TWO:
					productController.ShowAllProductsToAdmin();
					break;

				case Choice_Constants.THREE:
					orderController.ShowAllOrders();
					break;

				case Choice_Constants.FOUR:
					productController.UpdateProductDetails();
					break;

				case Choice_Constants.FIVE:
					orderController.ChangeOrderStatus();
					break;

				case Choice_Constants.SIX:
					userController.DisplayInfo(adminId);
					break;

				case Choice_Constants.SEVEN:
					userController.EditProfile(adminId);
					break;

				case Choice_Constants.EIGHT:
					Loader.Loader.RunLogout();
					Console.Clear();
					return; //return if user prompts to logout

				case Constants.Constants.BACK:
					Console.WriteLine("Cannot go back from admin menu. Please logout.");
					Thread.Sleep(1200);
					Console.Clear();
					break;

				default:
					Console.WriteLine("Enter valid choice.");
					Thread.Sleep(500);
					Console.Clear();
					break;
			}
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

			var choice = Console.ReadLine();

			switch (choice)
			{
				case Choice_Constants.ONE:
					productController.ShowAllProductsToCustomer();
					break;

				case Choice_Constants.TWO:
					orderController.PlaceOrder(customerId);
					break;

				case Choice_Constants.THREE:
					orderController.ShowAllOrdersByUserId(customerId);
					break;

				case Choice_Constants.FOUR:
					orderController.CancelOrder(customerId);
					break;

				case Choice_Constants.FIVE:
					userController.DisplayInfo(customerId);
					break;

				case Choice_Constants.SIX:
					userController.EditProfile(customerId);
					break;

				case Choice_Constants.SEVEN:
					orderController.DisplayOrderStatus(customerId);
					break;

				case Choice_Constants.EIGHT:
					Loader.Loader.RunLogout();
					Console.Clear();
					return; //return if user prompts to logout

				case Constants.Constants.BACK:
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
		}
	}
}