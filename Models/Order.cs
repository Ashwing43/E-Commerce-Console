using Models;
using System.Net;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace ECommerce.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> ProductIds { get; set; } = new List<Guid>();
        public decimal TotalAmount { get; set; }
        public Address Address { get; set; }
        public OrderStatus OrderStatus { set; get; }
    }
}
