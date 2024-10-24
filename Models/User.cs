using Models;
using System.Text.Json.Serialization;

namespace ECommerce.Models
{
    public enum UserRole { Admin, Customer }

    public abstract class User
    { 
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public abstract void DisplayInfo();
    }
}

