using Constants;
using ECommerce.Models;
using ECommerce.Services;
using System.Globalization;
using System.Security.Cryptography;

namespace ECommerce.Controllers
{
	public class ProductController
	{
		private readonly IProductService _productService;
		private readonly IUserService _userService;

		public ProductController(IProductService productService, IUserService userService)
		{
			_productService = productService;
			_userService = userService;
		}

		public void AddProduct(Guid userId)
		{
			try
			{
				var user = _userService.GetUserById(userId);

				if (user.Role != UserRole.Admin)
				{
					Console.WriteLine("Only admins can add products.");
					return;
				}

				Console.WriteLine("Enter Product Name:");
				var name = "";
				while (string.IsNullOrWhiteSpace(name))
				{
					name = Console.ReadLine();
					if (name == Constants.Constants.BACK)
					{
						Console.Clear();
						return;
					}
					if (string.IsNullOrWhiteSpace(name))
					{
						Console.WriteLine("Name cannot be empty. Please try again.");
					}
				}

				Console.WriteLine("Enter Product Price:");
				decimal price = 0;
				while (true)
				{
					var priceInput = Console.ReadLine();
					if (priceInput == Constants.Constants.BACK)
					{
						Console.Clear();
						return;
					}
					if (string.IsNullOrWhiteSpace(priceInput))
					{
						Console.WriteLine("Price cannot be empty, Please try again.");
						continue;
					}
					if (decimal.TryParse(priceInput, out price) && price > 0)
					{
						break;
					}
					Console.WriteLine("Please enter numeric value only greater than zero.");
				}

				Console.WriteLine("Enter Quantity:");
				var quantity = 0;
				while (true)
				{
					var quantityInput = Console.ReadLine();
					if (quantityInput == Constants.Constants.BACK)
					{
						Console.Clear();
						return;
					}
					if (string.IsNullOrWhiteSpace(quantityInput))
					{
						Console.WriteLine("Quantity cannot be empty, Please try again.");
						continue;
					}
					if (int.TryParse(quantityInput, out quantity) && quantity > 0)
					{
						break;
					}
					Console.WriteLine("Please enter numeric value only greater than zero.");
				}

				var product = _productService.GetAllProducts().FirstOrDefault(p => p.Name == name);

				if (product is null)
				{
					product = new Product() { Id = Guid.NewGuid(), Name = name, Price = price, Quantity = quantity };
					_productService.AddProduct(product);
					Console.WriteLine("Product added successfully.");
				}
				else
				{
					Console.WriteLine("Product already exists, cannot be added");
				}
				Loader.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		public void ShowAllProductsToCustomer()
		{
			try
			{
				var products = _productService.GetAllProducts().Where(prod => prod.Quantity > 0).ToList();

				if (products.Count <= 0)
				{
					Console.WriteLine("No products available.");
					Loader.Loader.PressAnyKeyToExit();
					return;
				}
				for (int i = 0; i < products.Count; i++)
				{
					Console.Write($"Sr. No. {i + 1},  ");
					products[i].DisplayInfo();
				}
				Loader.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		public void ShowAllProductsToAdmin()
		{
			try
			{
				var products = _productService.GetAllProducts().ToList();
				if (products.Count <= 0)
				{
					Console.WriteLine("No products available.");
					Loader.Loader.PressAnyKeyToExit();
					return;
				}
				for (int i = 0; i < products.Count; i++)
				{
					Console.Write($"Sr. No. {i + 1},  ");
					products[i].DisplayInfo();
				}
				Loader.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		public void UpdateProductDetails()
		{
			try
			{
				var products = _productService.GetAllProducts().ToList();
				if (products.Count <= 0)
				{
					Console.WriteLine("No products available.");
					Loader.Loader.PressAnyKeyToExit();
					return;
				}

				for (int i = 0; i < products.Count; i++)
				{
					Console.Write($"Sr. No. {i + 1},  ");
					products[i].DisplayInfo();
				}
				Console.WriteLine();

				Console.WriteLine("Enter sr. no. of product to update");
				var choice = "";
				int input = -1;
				while (string.IsNullOrWhiteSpace(choice))
				{
					choice = Console.ReadLine();
					if (choice == Constants.Constants.BACK)
					{
						Console.Clear();
						return;
					}
					if (string.IsNullOrWhiteSpace(choice))
					{
						Console.WriteLine("Input cannot be empty.");
						continue;
					}
					if (int.TryParse(choice, out input))
					{
						if (input < 1 || input > products.Count)
						{
							Console.WriteLine("Enter valid choice.");
							choice = "";
							continue;
						}
						break;
					}
					Console.WriteLine("Enter valid choice.");
					choice = "";
				}
				Product prod = _productService.GetProductById(products[input - 1].Id);


				Console.WriteLine("Enter a choice");
				Console.WriteLine("1. Add quantity");
				Console.WriteLine("2. Reduce quantity");
				Console.WriteLine("3. Update amount");

				choice = "";
				while (string.IsNullOrWhiteSpace(choice))
				{
					choice = Console.ReadLine();
					if (choice == Constants.Constants.BACK)
					{
						Console.Clear();
						return;
					}
					if (string.IsNullOrWhiteSpace(choice))
					{
						Console.WriteLine("Choice cannot be empty");
						continue;
					}
					if (choice == "1" || choice == "2" || choice == "3")
					{
						break;
					}
					Console.WriteLine("Enter valid choice.");
					choice = "";
				}

				if (choice == Choice_Constants.ONE)
				{
					Console.WriteLine("Enter quantity to increase");
					int quantity = 0;
					while (true)
					{
						var quantityInput = Console.ReadLine();
						if (quantityInput == Constants.Constants.BACK)
						{
							Console.Clear();
							return;
						}
						if (int.TryParse(quantityInput, out quantity) && quantity > 0)
						{
							break;
						}
						Console.WriteLine("Enter numeric value greater than 0");
					}
					prod.Quantity += quantity;
					Console.WriteLine("Quantity increased by " + quantity);
				}
				else if (choice == Choice_Constants.TWO)
				{
					Console.WriteLine("Enter quantity to decrease");

					int quantity = 0;
					while (true)
					{
						var inp = Console.ReadLine();
						if (inp == Constants.Constants.BACK)
						{
							Console.Clear();
							return;
						}
						if (int.TryParse(inp, out int a) && a > 0)
						{
							quantity = a;
							break;
						}
						Console.WriteLine("Enter numeric value greater than 0");
					}

					if (prod.Quantity < quantity)
					{
						Console.WriteLine("Quantity cannot be reduced as value is greater than existing quantity.");
						Loader.Loader.PressAnyKeyToExit();
						return;
					}
					prod.Quantity -= quantity;
					Console.WriteLine("Quantity decreased by " + quantity);
				}
				else if (choice == "3")
				{
					Console.WriteLine("Enter new amount");
					int newAmount = 0;
					while (true)
					{
						var inp = Console.ReadLine();
						if (inp == Constants.Constants.BACK)
						{
							Console.Clear();
							return;
						}
						if (int.TryParse(inp, out int a) && a > 0)
						{
							newAmount = a;
							break;
						}
						Console.WriteLine("Enter numeric value greater than 0");
					}
					prod.Price = newAmount;
					Console.WriteLine("New amount is " + newAmount);
				}
				Loader.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}
	}
}
