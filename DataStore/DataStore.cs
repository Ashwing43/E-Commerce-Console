using ECommerce.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DataStorage
{
	public static class DataStore
	{
		public static List<User> Users = new List<User>();
		public static List<Product> Products = new List<Product>();
		public static List<Order> Orders = new List<Order>();

		private static readonly string UsersFilePath = "Users.json";
		private static readonly string ProductsFilePath = "Products.json";
		private static readonly string OrdersFilePath = "Orders.json";

		public static void SaveData()
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				Converters = { new JsonStringEnumConverter(), new UserJsonConverter() }
			};

			File.WriteAllText(UsersFilePath, JsonSerializer.Serialize(Users, options));
			File.WriteAllText(ProductsFilePath, JsonSerializer.Serialize(Products, options));
			File.WriteAllText(OrdersFilePath, JsonSerializer.Serialize(Orders, options));
		}

		public static void LoadData()
		{
			var options = new JsonSerializerOptions
			{
				Converters = { new JsonStringEnumConverter(), new UserJsonConverter() }
			};

			if (File.Exists(UsersFilePath))
			{
				Users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(UsersFilePath), options) ?? new List<User>();
			}

			if (File.Exists(ProductsFilePath))
			{
				Products = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(ProductsFilePath), options) ?? new List<Product>();
			}

			if (File.Exists(OrdersFilePath))
			{
				Orders = JsonSerializer.Deserialize<List<Order>>(File.ReadAllText(OrdersFilePath), options) ?? new List<Order>();
			}
		}
	}
}
