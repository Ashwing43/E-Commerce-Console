using ECommerce.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DataStorage
{
    public static class DataStore
    {
        public static List<User> Users = new List<User>();
        public static List<Product> Products = new List<Product>();
        public static List<Order> Orders = new List<Order>();
    }
}
