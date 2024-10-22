using Models.Contract;
using System.Text.Json.Serialization;

namespace ECommerce.Models
{
    public class Product : IProduct
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public void DisplayInfo()
        {
            Console.WriteLine($"Name: {this.Name}, Price: {this.Price}, Quantity: {this.Quantity}");
        }
    }
}
