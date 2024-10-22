using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Customer : User
    {
        #region Fields
        public List<Address> Addresses { get; set; } = new List<Address>();
        
        public Queue<string> Notifications { get; set; } = new Queue<string>();
        #endregion

        #region Methods
        public override void DisplayInfo()
        {
            Console.WriteLine("\nCustomer Info: ");
            Console.WriteLine($"Customer name : {this.Name}");
            Console.WriteLine($"Customer email : {this.Email}");
        }
        #endregion
    }
}