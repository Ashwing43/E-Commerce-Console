using CustomLogger;
using CustomExceptions;
using ECommerce.Models;
using ECommerce.Repositories;
using System.Xml.Serialization;
using Delegates;


namespace ECommerce.Services
{
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

		public void ChangeOrderStatus()
		{
			var orders = _orderRepository.GetAll().Where(o => o.OrderStatus != OrderStatus.Cancelled && o.OrderStatus != OrderStatus.Delivered).ToList();

			if (orders.Count < 1)
			{
				Console.WriteLine("No orders at the moment.");
				Loader.Loader.PressAnyKeyToExit();
				return;
			}

			foreach (Order o in orders)
			{
				Console.WriteLine($"Order Id: {o.Id},  User Id: {o.UserId},  Amount:{o.TotalAmount},  Order Status: {o.OrderStatus}");
			}
			Console.WriteLine("Enter order id");
			(Guid OrderId, bool wantToGoBack) = GetOrderIdInput();
			if (wantToGoBack)
			{
				Console.Clear();
				return;
			}

			try
			{
				Order order = _orderRepository.GetById(OrderId);
				if (order is null)
				{
					throw new OrderNotFoundException();
				}

				Console.WriteLine("Change status to?");
				Console.WriteLine("1. Processed");
				Console.WriteLine("2. Shipped");
				Console.WriteLine("3. Delivered");
				OrderStatus newStatus = OrderStatus.Pending;
				bool isStatusChanged = false;
				(var input, wantToGoBack) = GetChoiceInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				switch (input)
				{
					case Constants.Choice_Constants.ONE:
						newStatus = OrderStatus.Processed;
						break;

					case Constants.Choice_Constants.TWO:
						newStatus = OrderStatus.Shipped;
						break;

					case Constants.Choice_Constants.THREE:
						newStatus = OrderStatus.Delivered;
						break;
				}

				string status = "";
				switch (input)
				{
					case Constants.Choice_Constants.ONE:
						if (order.OrderStatus == OrderStatus.Pending)
						{
							order.OrderStatus = OrderStatus.Processed;
							status = OrderStatus.Processed.ToString().ToLower();
							isStatusChanged = true;
						}
						else
						{
							Console.WriteLine($"Cannot change the status. The order is already {order.OrderStatus}.");
						}
						break;

					case Constants.Choice_Constants.TWO:
						if (order.OrderStatus == OrderStatus.Processed)
						{
							order.OrderStatus = OrderStatus.Shipped;
							status = OrderStatus.Shipped.ToString().ToLower();
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

					case Constants.Choice_Constants.THREE:
						if (order.OrderStatus == OrderStatus.Shipped)
						{
							order.OrderStatus = OrderStatus.Delivered;
							status = OrderStatus.Delivered.ToString().ToLower();
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
					if (OnOrderProcessed != null)
					{
						OnOrderProcessed.Invoke(order, status);
					}
					_orderRepository.Save();
				}
				Loader.Loader.PressAnyKeyToExit();
			}
			catch (OrderNotFoundException ex)
			{
				Console.WriteLine("Order cannot be found");
				Logger.LogError(ex);

				Loader.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		public void Save()
		{
			_orderRepository.Save();
		}

		public OrderProcessedEventHandler OnOrderProcessed { set; get; }

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

		private (Guid, bool) GetOrderIdInput()
		{
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
				if (Guid.TryParse(input, out Guid OrderId))
				{
					return (OrderId, false);
				}
				Console.WriteLine("Invalid id format");
			}
		}

		private (string, bool) GetChoiceInput()
		{
			while (true)
			{
				var input = Console.ReadLine();
				if (input == Constants.Constants.BACK)
				{
					Console.Clear();
					return (input, true);
				}
				if (input == Constants.Choice_Constants.ONE || input == Constants.Choice_Constants.TWO || input == Constants.Choice_Constants.THREE || input == Constants.Choice_Constants.FOUR)
				{
					return (input, false);
				}
				else
				{
					Console.WriteLine("Enter valid choice");
				}
			}
		}
	}
}