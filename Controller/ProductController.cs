using AppConstants;
using ECommerce.Models;
using ECommerce.Services;
using Models.Contract;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Xml.Linq;

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

				bool wantToGoBack;
				Console.WriteLine("Enter Product Name:");
				(var name, wantToGoBack) = GetNameInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				Console.WriteLine("Enter Product Price:");
				(decimal price, wantToGoBack) = GetPriceInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				Console.WriteLine("Enter Quantity:");
				(var quantity, wantToGoBack) = GetQuantityInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
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
				Animation.Loader.PressAnyKeyToExit();
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
					Animation.Loader.PressAnyKeyToExit();
					return;
				}
				for (int i = 0; i < products.Count; i++)
				{
					Console.Write($"Sr. No. {i + 1},\t");
					products[i].DisplayInfo();
				}
				Animation.Loader.PressAnyKeyToExit();
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
					Animation.Loader.PressAnyKeyToExit();
					return;
				}
				for (int i = 0; i < products.Count; i++)
				{
					Console.Write($"Sr. No. {i + 1},\t ");
					products[i].DisplayInfo();
				}
				Animation.Loader.PressAnyKeyToExit();
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
					Animation.Loader.PressAnyKeyToExit();
					return;
				}

				for (int i = 0; i < products.Count; i++)
				{
					Console.Write($"Sr. No. {i + 1},\t");
					products[i].DisplayInfo();
				}
				Console.WriteLine();

				Console.WriteLine("Enter sr. no. of product to update");

				var choice = "";
				(int input, bool wantToGoBack) = GetSerialNumberInput(products.Count);
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				Product prod = _productService.GetProductById(products[input - 1].Id);

				Console.WriteLine("Enter a choice");
				Console.WriteLine("1. Add quantity");
				Console.WriteLine("2. Reduce quantity");
				Console.WriteLine("3. Update price");

				(choice, wantToGoBack) = GetChoiceInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				if (choice == ChoiceConstants.ONE)
				{
					Console.WriteLine("Enter quantity to increase");
					(int quantity, wantToGoBack) = GetQuantityInput();
					if (wantToGoBack)
					{
						Console.Clear();
						return;
					}

					prod.Quantity += quantity;
					_productService.Save();
					Console.WriteLine("Quantity increased by " + quantity);
					Animation.Loader.PressAnyKeyToExit();
					return;
				}

				if (choice == ChoiceConstants.TWO)
				{
					Console.WriteLine("Enter quantity to decrease");

					(int quantity, wantToGoBack) = GetQuantityInput();
					if (wantToGoBack)
					{
						Console.Clear();
						return;
					}

					if (prod.Quantity < quantity)
					{
						Console.WriteLine("Quantity cannot be reduced as value is greater than existing quantity.");
						Animation.Loader.PressAnyKeyToExit();
						return;
					}

					prod.Quantity -= quantity;
					_productService.Save();
					Console.WriteLine("Quantity decreased by " + quantity);
					Animation.Loader.PressAnyKeyToExit();
					return;
				}

				if (choice == ChoiceConstants.THREE)
				{
					Console.WriteLine("Enter new price");
					(decimal newPrice, wantToGoBack) = GetPriceInput();
					if (wantToGoBack)
					{
						Console.Clear();
						return;
					}

					prod.Price = newPrice;
					_productService.Save();
					Console.WriteLine("New amount is " + newPrice);
					Animation.Loader.PressAnyKeyToExit();
					return;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		private (decimal, bool) GetPriceInput()
		{
			decimal price = 0;
			while (true)
			{
				var priceInput = Console.ReadLine();
				if (priceInput == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (0, true);
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
			return (price, false);
		}

		private (int, bool) GetQuantityInput()
		{
			var quantity = 0;
			while (true)
			{
				var quantityInput = Console.ReadLine();
				if (quantityInput == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (quantity, true);
				}
				if (string.IsNullOrWhiteSpace(quantityInput))
				{
					Console.WriteLine("Quantity cannot be empty, Please try again.");
					continue;
				}
				if (int.TryParse(quantityInput, out quantity) && quantity > 0)
				{
					return (quantity, false);
				}
				Console.WriteLine("Please enter numeric value only greater than zero.");
			}
		}

		private (string, bool) GetNameInput()
		{
			var name = "";
			while (string.IsNullOrWhiteSpace(name))
			{
				name = Console.ReadLine();
				if (name == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (name, true);
				}
				if (string.IsNullOrWhiteSpace(name))
				{
					Console.WriteLine("Name cannot be empty. Please try again.");
				}
			}
			return (name, false);
		}
		private (int, bool) GetSerialNumberInput(int productsCount)
		{
			int input = -1;
			while (true)
			{
				var choice = Console.ReadLine();
				if (choice == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (input, true);
				}
				if (string.IsNullOrWhiteSpace(choice))
				{
					Console.WriteLine("Input cannot be empty.");
					continue;
				}
				if (int.TryParse(choice, out input))
				{
					if (input < 1 || input > productsCount)
					{
						Console.WriteLine("Enter valid choice.");
						continue;
					}
					break;
				}
				Console.WriteLine("Enter valid choice.");
			}
			return (input, false);
		}

		private (string, bool) GetChoiceInput()
		{
			while (true)
			{
				var choice = Console.ReadLine();
				if (choice == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (choice, true);
				}
				if (string.IsNullOrWhiteSpace(choice))
				{
					Console.WriteLine("Choice cannot be empty");
					continue;
				}
				if (choice == ChoiceConstants.ONE || choice == ChoiceConstants.TWO || choice == ChoiceConstants.THREE)
				{
					return (choice, false);
				}
				Console.WriteLine("Enter valid choice.");
			}
		}
	}
}
