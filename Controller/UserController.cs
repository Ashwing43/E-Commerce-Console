using Factories;
using Models;
using ECommerce.Models;
using ECommerce.Services;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Text;
using System.Xml.Serialization;
using Utils;
using CustomLogger;

namespace ECommerce.Controllers
{
    public class UserController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public void SignUp()
        {
            try
            {
                Console.WriteLine("Sign up as: 1. Admin  2. Customer");
                string choice = "";
                bool flag = true;
                while (string.IsNullOrWhiteSpace(choice) || flag)
                {
                    choice = Console.ReadLine();
                    if(choice == "back") {
                        Console.Clear();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(choice) || !(choice == "1" || choice == "2"))
                    {
                        Console.WriteLine("Enter valid choice.");
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }

                var userRole = choice == "1" ? UserRole.Admin : UserRole.Customer;

                Console.WriteLine("Enter Name:");
                var name = "";

                while (string.IsNullOrWhiteSpace(name))
                {
                    name = Console.ReadLine();
                    if(name == "back") {
                        Console.Clear();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.WriteLine("Name cannot be empty. Please try again.");
                    }
                }

                Console.WriteLine("Enter Email:");
                var email = "";
                while (string.IsNullOrWhiteSpace(email) || !email.IsValidEmail())
                {
                    email = Console.ReadLine();
                    if(email == "back") {
                        Console.Clear();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        Console.WriteLine("Email cannot be empty. Please try again.");
                    }
                    else if (!email.IsValidEmail())
                    {
                        Console.WriteLine("Please provide valid email.");
                    }
                    if (_userService.GetAllUsers().FirstOrDefault(u => u.Email == email) != null)
                    {
                        Console.WriteLine("Email Id Exists. Try with another email Id");
                        email = "";
                    }
                }

                Console.WriteLine("Enter Password:");
                var password = "";
                while (string.IsNullOrWhiteSpace(password))
                {
                    password = Utils.PasswordReader.ReadPassword();
                    if(password == "back") return;
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Password cannot be empty. Please try again.");
                    }
                    else if (!password.IsValidPassword())
                    {
                        Console.WriteLine("\nPassword length must be atleast 8 with 1 Uppercase character, 1 symbol and a numeric character");
                        password = "";
                    }
                }

                User user = UserFactory.CreateUser(name, email, password, userRole);

                _userService.AddUser(user);
                Console.WriteLine($"\n{userRole} account created successfully.");
                Console.WriteLine("\nPress any key to continue.");
                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public User Login()
        {
            try
            {
                Console.WriteLine("Enter Email for login:");
                var email = "";
                while (string.IsNullOrWhiteSpace(email))
                {
                    email = Console.ReadLine();
                    if(email == "back") {
                        Console.Clear();
                        return null;
                    }
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        Console.WriteLine("Email cannot be empty. Please try again.");
                    }
                }

                Console.WriteLine("Enter Password:");
                var password = "";
                while (string.IsNullOrWhiteSpace(password))
                {
                    password = Utils.PasswordReader.ReadPassword();
                    if(password == "back") return null;

                    if (string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Password cannot be empty. Please try again.");
                    }
                }

                var users = _userService.GetAllUsers();
                var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                {
                    return null;
                }

                Console.WriteLine($"\nLogged in as {user.Role}");
                Console.WriteLine("\nPress any key to continue.");
                Console.ReadKey();
                Console.Clear();
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
                return null;
            }
        }

        public void DisplayInfo(Guid id)
        {
            try
            {
                _userService.DisplayInfo(id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public void EditProfile(Guid userId)
        {
            try
            {
                User user = _userService.GetUserById(userId);
                Console.WriteLine("Enter a choice to edit:");
                Console.WriteLine("1. Name");
                Console.WriteLine("2. Email");
                Console.WriteLine("3. Password");

                var input = "";
                while (string.IsNullOrWhiteSpace(input))
                {
                    input = Console.ReadLine();
                    if(input == "back") {
                        Console.Clear();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(input) || !(input == "1" || input == "2" || input == "3"))
                    {
                        Console.WriteLine("Enter valid choice");
                        input = "";
                    }
                }

                if (input == "1")
                {
                    Console.WriteLine("Enter new name");
                    var name = "";
                    while (string.IsNullOrWhiteSpace(name))
                    {
                        name = Console.ReadLine();
                        if(name == "back") {
                            Console.Clear();
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine("Input cannot be empty.");
                        }
                    }
                    user.Name = name;
                    Console.WriteLine("Name changed successfully");
                }
                else if (input == "2")
                {
                    Console.WriteLine("Enter new Email");
                    var email = "";
                    while (string.IsNullOrWhiteSpace(email))
                    {
                        email = Console.ReadLine();
                        if(email == "back") {
                            Console.Clear();
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(email))
                        {
                            Console.WriteLine("Input cannot be empty");
                        }
                        else if (_userService.GetAllUsers().FirstOrDefault(u => u.Email == email) != null)
                        {
                            Console.WriteLine("Email id exist in our database.\nTry with another email.");
                            email = "";
                        }
                    }
                    user.Email = email;
                    Console.WriteLine("Email id changed successfully.");
                }
                else if (input == "3")
                {
                    Console.WriteLine("Enter new Password:");
                    var newPassword = "";
                    while (string.IsNullOrWhiteSpace(newPassword))
                    {
                        newPassword = Utils.PasswordReader.ReadPassword();
                        if(newPassword == "back") return;
                        if (string.IsNullOrWhiteSpace(newPassword))
                        {
                            Console.WriteLine("Password cannot be empty. Please try again.");
                        }
                        else if (!newPassword.IsValidPassword())
                        {
                            Console.WriteLine("\nPassword must have 1 Uppercase character, 1 symbol and a numeric character");
                            newPassword = "";
                        }
                    }

                    Console.WriteLine("\nEnter old Password:");
                    var password = "";
                    while (string.IsNullOrWhiteSpace(password))
                    {
                        password = Utils.PasswordReader.ReadPassword();
                        if(password == "back") return;
                        if (string.IsNullOrWhiteSpace(password))
                        {
                            Console.WriteLine("Password cannot be empty. Please try again.");
                        }
                    }

                    if (password == user.Password)
                    {
                        user.Password = newPassword;
                        Console.WriteLine("\nPassword changed successfully");
                    }
                    else
                    {
                        Console.WriteLine("\nOld password did not match.");
                    }
                }
                Console.WriteLine("Press any key to return to menu");
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
