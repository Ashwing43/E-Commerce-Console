using ECommerce.Models;
using ECommerce.Services;
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
                    if(name == Constants.Constants.BACK) {
                        Console.Clear();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.WriteLine("Name cannot be empty. Please try again.");
                    }
                }

                Console.WriteLine("Enter Product Price:");
                var priceInput = "";
                decimal price = 0;
                while (string.IsNullOrWhiteSpace(priceInput))
                {
                    priceInput = Console.ReadLine();
                    if(priceInput == Constants.Constants.BACK) {
                        Console.Clear();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(priceInput))
                    {
                        Console.WriteLine("Price cannot be empty, Please try again.");
                    }
                    else if (decimal.TryParse(priceInput, out decimal p) && p > 0)
                    {
                        price = p;
                    }
                    else
                    {
                        Console.WriteLine("Please enter numeric value only greater than zero.");
                        priceInput = "";
                    }
                }

                Console.WriteLine("Enter Quantity:");
                var quantityInput = "";
                var quantity = 0;
                while (string.IsNullOrWhiteSpace(quantityInput))
                {
                    quantityInput = Console.ReadLine();
                    if(quantityInput == Constants.Constants.BACK) {
                        Console.Clear();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(quantityInput))
                    {
                        Console.WriteLine("Quantity cannot be empty, Please try again.");
                    }
                    else if (int.TryParse(quantityInput, out int q) && q > 0)
                    {
                        quantity = q;
                    }
                    else
                    {
                        Console.WriteLine("Please enter numeric value only greater than zero.");
                        quantityInput = "";
                    }
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
                Console.WriteLine("\nPress any key to return to menu.");
                Console.ReadKey();
                Console.Clear();
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public void ShowAllProductsCustomer()
        {
            try
            {
                var products = _productService.GetAllProducts().Where(prod => prod.Quantity > 0).ToList();
                if (products.Count <= 0)
                {
                    Console.WriteLine("No products available.");
                    Console.WriteLine("\nPress any key to return to menu.");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                int i = 1;
                foreach (var product in products)
                {
                    Console.Write($"Sr. No. {i++},  ");
                    product.DisplayInfo();
                }
                Console.WriteLine("\nPress any key to return to menu.");
                Console.ReadKey();
                Console.Clear();
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public void ShowAllProductsAdmin()
        {
            try
            {
                var products = _productService.GetAllProducts().ToList();
                if (products.Count <= 0)
                {
                    Console.WriteLine("No products available.");
                    Console.WriteLine("\nPress any key to return to menu.");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                int i = 1;
                foreach (var product in products)
                {
                    Console.Write($"Sr. No. {i++},  ");
                    product.DisplayInfo();
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

        public void UpdateProductDetails()
        {
            try
            {
                var products = _productService.GetAllProducts().ToList();
                if (products.Count <= 0)
                {
                    Console.WriteLine("No products available.");
                    Console.WriteLine("\nPress any key to return to menu.");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                int i = 1;
                foreach (var p in products)
                {
                    Console.Write($"Sr. No. {i++},  ");
                    p.DisplayInfo();
                }
                Console.WriteLine();
                Console.WriteLine("Enter sr. no. of product to update");
                var choice = "";
                int input = -1;
                while (string.IsNullOrWhiteSpace(choice))
                {
                    choice = Console.ReadLine();
                    if(choice == Constants.Constants.BACK) {
                        Console.Clear();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(choice))
                    {
                        Console.WriteLine("Input cannot be empty.");
                    }
                    else if (int.TryParse(choice, out int a))
                    {
                        input = a;
                        if (a < 1 || a > products.Count)
                        {
                            Console.WriteLine("Enter valid choice.");
                            choice = "";
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter valid choice.");
                        choice = "";
                    }
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
                    if(choice == Constants.Constants.BACK) {
                        Console.Clear();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(choice))
                    {
                        Console.WriteLine("Choice cannot be empty");
                    }
                    else if (choice == "1" || choice == "2" || choice == "3")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Enter valid choice.");
                        choice = "";
                    }
                }

                if (choice == "1")
                {
                    Console.WriteLine("Enter quantity to increase");
                    int value = 0;
                    while (true)
                    {
                        var inp = Console.ReadLine();
                        if(inp == Constants.Constants.BACK) {
                            Console.Clear();
                            return;
                        }
                        if (int.TryParse(inp, out int a) && a > 0)
                        {
                            value = a;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Enter numeric value greater than 0");
                        }
                    }
                    prod.Quantity += value;
                    Console.WriteLine("Quantity increased by " + value);
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Enter quantity to decrease");
                    int value = 0;
                    while (true)
                    {
                        var inp = Console.ReadLine();
                        if(inp == Constants.Constants.BACK) {
                            Console.Clear();
                            return;
                        }
                        if (int.TryParse(inp, out int a) && a > 0)
                        {
                            value = a;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Enter numeric value greater than 0");
                        }
                    }
                    if (prod.Quantity < value)
                    {
                        Console.WriteLine("Quantity cannot be reduced as value is greater than existing quantity.");
                        Console.WriteLine("\nPress any key to return to menu.");
                        Console.ReadKey();
                        Console.Clear();
                        return;
                    }
                    prod.Quantity -= value;
                    Console.WriteLine("Quantity decreased by " + value);
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Enter new amount");
                    int value = 0;
                    while (true)
                    {
                        var inp = Console.ReadLine();
                        if(inp == Constants.Constants.BACK) {
                            Console.Clear();
                            return;
                        }
                        if (int.TryParse(inp, out int a) && a > 0)
                        {
                            value = a;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Enter numeric value greater than 0");
                        }
                    }
                    prod.Price = value;
                    Console.WriteLine("New amount is " + value);
                }
                Console.WriteLine("\nPress any key to return to menu.");
                Console.ReadKey();
                Console.Clear();
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }
    }
}
