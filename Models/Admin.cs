using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Admin : User
    {
        #region Methods
        public override void DisplayInfo()
        {
            Console.WriteLine("\nAdmin Info: ");
            Console.WriteLine($"Admin name : {this.Name}");
            Console.WriteLine($"Admin email : {this.Email}");
        }
        #endregion

    }
}