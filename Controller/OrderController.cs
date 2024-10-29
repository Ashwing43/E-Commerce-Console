using Models;
using Models.Contract;
using ECommerce.Models;
using ECommerce.Services;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using CustomExceptions;
using ECommerce.Repositories;
using Constants;

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

			_orderService.OnOrderProcessed += HandleOrderProcessed;
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
				List<Product> products = _productService.GetAllProducts().ToList();

				DisplayAvailableProducts(products);

				var productSerialNumbers = new List<int>();
				List<Guid> productIds = new List<Guid>();
				decimal totalAmount = 0;

				while (true)
				{
					Console.WriteLine("Enter a choice");
					Console.WriteLine("1. Add product");
					Console.WriteLine("2. Finish order");

					(var choice, bool wantToGoBack) = GetChoiceInput();
					if (wantToGoBack)
					{
						Console.Clear();
						return;
					}


					if (choice == Choice_Constants.ONE)
					{
						Console.WriteLine("Enter Product sr. no. to order:");

						(int serialNumber, wantToGoBack) = GetSerialNumberInput(products.Count);
						if (wantToGoBack)
						{
							Console.Clear();
							return;
						}

						var p = _productService.GetProductById(products[serialNumber - 1].Id);
						totalAmount += p.Price;
						p.Quantity--;
						productIds.Add(products[serialNumber - 1].Id);
						Console.WriteLine($"Added product with sr no. {serialNumber}");
						Console.WriteLine($"Total amount: {totalAmount}\n");
						continue;
					}
					if (choice == Choice_Constants.TWO)
					{
						if (productIds.Count < 1)
						{
							Console.WriteLine("You have not selected anything.\nOrder cancelled.");
							Loader.Loader.PressAnyKeyToExit();
							return;
						}

						Console.WriteLine($"Your total amount for this order is {totalAmount}.");
						var addresses = user.Addresses;

						Address address;
						DisplayAddresses(addresses);
						Console.WriteLine($"{addresses.Count}. Enter new Address");

						(int addressChoice, wantToGoBack) = GetAddressChoiceInput(addresses.Count);
						if (wantToGoBack)
						{
							Console.Clear();
							return;
						}

						if (addressChoice == addresses.Count)
						{
							(address, wantToGoBack) = Utils.AddressReader.GetAddress();
							if (wantToGoBack)
							{
								Console.Clear();
								return;
							}
							addresses.Add(address);
						}
						else
						{
							address = addresses[addressChoice];
						}

						var order = new Order()
						{
							Id = Guid.NewGuid(),
							UserId = userId,
							ProductIds = productIds,
							TotalAmount = totalAmount,
							Address = address,
							OrderStatus = OrderStatus.Pending
						};

						_orderService.PlaceOrderAsync(order);
						Console.WriteLine("Order placed successfully.");

						Loader.Loader.PressAnyKeyToExit();
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

				DisplayAllOrder(orders);
				Loader.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		public void ShowAllOrdersByUserId(Guid userId)
		{
			try
			{
				var orders = _orderService.GetAllOrdersByUserId(userId).ToList();
				DisplayAllOrder(orders);
				Loader.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		public void DisplayOrderStatus(Guid userId)
		{
			try
			{
				var orders = _orderService.GetAllOrdersByUserId(userId);
				foreach (Order o in orders)
				{
					Console.WriteLine();
					Console.WriteLine($"Order Id: {o.Id},  Amount:{o.TotalAmount},  Order Status: {o.OrderStatus}");
					Console.WriteLine($"Products:");
					foreach (Guid prodId in o.ProductIds)
					{
						Product p = _productService.GetProductById(prodId);
						Console.WriteLine($"Name: {p.Name}, Price: {p.Price}");
					}
				}
				Console.WriteLine("\nEnter order Id to check status");

				(Guid id, bool wantToGoBack) = GetGuidInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				try
				{
					Order order = _orderService.GetOrderById(id);

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
					Loader.Loader.PressAnyKeyToExit();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		private void DisplayAllOrder(List<Order> orders)
		{
			if (orders.Count < 1)
			{
				Console.WriteLine($"No orders at the moment");
				return;
			}
			foreach (var order in orders)
			{
				Console.WriteLine($"\nOrder ID: {order.Id}, Total Amount: {order.TotalAmount}, Status: {order.OrderStatus}");
				Console.WriteLine("Products:");
				foreach (var productId in order.ProductIds)
				{
					Product product = _productService.GetProductById(productId);
					Console.WriteLine($"\tId: {product.Id}, Name: {product.Name}, Price: {product.Price}");
				}
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
				if (orders.Count < 1)
				{
					Console.WriteLine("No orders at the moment to cancel.");
					Loader.Loader.PressAnyKeyToExit();
					return;
				}
				foreach (var order in orders)
				{
					Console.WriteLine($"\nOrder ID: {order.Id}, Total Amount: {order.TotalAmount}, Status: {order.OrderStatus}");
				}

				Console.WriteLine("Enter order id to cancel:");

				(Guid id, bool wantToGoBack) = GetGuidInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				try
				{
					Order order = _orderService.GetOrderById(id);
					if (order is null)
					{
						throw new OrderNotFoundException();
					}

					if (order.OrderStatus == OrderStatus.Cancelled)
					{
						Console.WriteLine("The order is already cancelled.");
						return;
					}

					if (order.OrderStatus == OrderStatus.Delivered)
					{
						Console.WriteLine("The order is delivered. Cannot be cancelled.");
						return;
					}

					order.OrderStatus = OrderStatus.Cancelled;
					Console.WriteLine("Order has been cancelled.");
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

				Loader.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		private void HandleOrderProcessed(Order o, string newStatus)
		{
			try
			{
				Console.WriteLine($"Order {newStatus}");
				Guid userId = o.UserId;
				Customer c = (Customer)_userService.GetUserById(userId);
				c.Notifications.Enqueue($"Your order with order id {o.Id} has been {newStatus}.");
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		private (string, bool) GetChoiceInput()
		{
			var choice = "";
			while (true)
			{
				var input = Console.ReadLine();
				if (input == Constants.Constants.BACK)
				{
					Console.Clear();
					return (input, true);
				}
				if (string.IsNullOrWhiteSpace(input))
				{
					Console.WriteLine("Input cannot be empty.");
					continue;
				}
				if (input == Choice_Constants.ONE || input == Choice_Constants.TWO)
				{
					choice = input;
					break;
				}
				Console.WriteLine("Enter valid choice.");
			}
			return (choice, false);
		}

		private (int, bool) GetAddressChoiceInput(int addressCount)
		{
			int addressChoice = 0;
			while (true)
			{
				var input = Console.ReadLine();
				if (input == Constants.Constants.BACK)
				{
					Console.Clear();
					return (-1, true);
				}
				if (!int.TryParse(input, out addressChoice))
				{
					Console.WriteLine("Enter valid input.");
					continue;
				}
				if (addressChoice < 0 || addressChoice > addressCount)
				{
					Console.WriteLine("Enter valid input.");
					continue;
				}
				break;
			}
			return (addressChoice, false);
		}

		private (int, bool) GetSerialNumberInput(int productsListLength)
		{

			int serialNumber = -1;
			while (true)
			{
				var input = Console.ReadLine();
				if (input == Constants.Constants.BACK)
				{
					Console.Clear();
					return (-1, true);
				}
				if (string.IsNullOrWhiteSpace(input))
				{
					Console.WriteLine("Input cannot be empty.");
					continue;
				}
				if (!int.TryParse(input, out serialNumber))
				{
					Console.WriteLine("Enter valid input.");
					continue;
				}
				if (serialNumber < 1 || serialNumber > productsListLength)
				{
					Console.WriteLine($"Enter value between 1 and {productsListLength}");
					continue;
				}
				break;
			}
			return (serialNumber, false);
		}

		private (Guid, bool flag) GetGuidInput()
		{
			Guid id;
			while (true)
			{
				var input = Console.ReadLine();
				if (input == Constants.Constants.BACK)
				{
					Console.Clear();
					return (new Guid(), true);
				}
				if (string.IsNullOrWhiteSpace(input))
				{
					Console.WriteLine("Input cannot be empty.");
					continue;
				}
				if (Guid.TryParse(input, out id))
				{
					break;
				}
				Console.WriteLine("Invalid id format.");
			}
			return (id, false);
		}

		private void DisplayAvailableProducts(List<Product> products)
		{
			Console.WriteLine("Available products");

			if (products.Count <= 0)
			{
				Console.WriteLine("No products available");
				return;
			}

			for (int i = 0; i < products.Count; i++)
			{
				Console.Write($"sr no. {i + 1},  ");
				products[i].DisplayInfo();
			}
			Console.WriteLine();
		}

		private void DisplayAddresses(List<Address> addresses)
		{
			Console.WriteLine("\nChoose address");
			for (int j = 0; j < addresses.Count; j++)
			{
				Console.WriteLine($"{j}. {addresses[j].Street}, {addresses[j].City}, {addresses[j].ZipCode}");
			}
		}
	}
}