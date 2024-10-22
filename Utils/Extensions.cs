using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils
{
    public static class Extensions
    {
        private static string emailPattern = "^[a-zA-Z0-9.]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$";
        private static string passwordPattern = "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
        private static string zipcodePattern = "^\\d{6}$";
        public static bool IsValidEmail(this string s)
        {
            return Regex.IsMatch(s, emailPattern);
        }

        public static bool IsValidPassword(this string s)
        {
            return Regex.IsMatch(s, passwordPattern);
        }

        public static bool IsValidZip(this string s)
        {
            return Regex.IsMatch(s, zipcodePattern);
        }
    }
}
