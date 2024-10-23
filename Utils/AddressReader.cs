using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Utils;
using ECommerce.Models; 

namespace Utils
{
    public static class AddressReader
    {
        public static Address GetAddress()
        {
            Console.WriteLine("Enter adress details");
            Console.Write("Enter Street: ");
            string street = "";
            while (string.IsNullOrWhiteSpace(street))
            {
                street = Console.ReadLine();
                if(street == "back") return new Address(){Street = "back"};
                if (string.IsNullOrWhiteSpace(street))
                {
                    Console.WriteLine("Input cannot be empty.");
                }
            }

            Console.Write("Enter city: ");
            string city = "";

            while (string.IsNullOrWhiteSpace(city))
            {
                city = Console.ReadLine();
                if(city == "back") return new Address(){Street = "back"};
                if (string.IsNullOrWhiteSpace(city))
                {
                    Console.WriteLine("Input cannot be empty.");
                }
            }

            Console.Write("Enter zipcode: ");
            string zip = "";

            while (string.IsNullOrWhiteSpace(zip))
            {
                zip = Console.ReadLine();
                if(zip == "back") return new Address(){Street = "back"};
                if (string.IsNullOrWhiteSpace(zip))
                {
                    Console.WriteLine("Input cannot be empty.");
                }
                else if (!zip.IsValidZip())
                {
                    Console.WriteLine("Enter zip code of 6 digits, without any spaces.");
                    zip = "";
                }
            }

            return new Address()
            {
                Street = street,
                City = city,
                ZipCode = zip
            };
        }
    }
}
