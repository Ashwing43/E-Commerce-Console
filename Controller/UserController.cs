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
using AppConstants;
using System.Reflection.Metadata.Ecma335;

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
				(string choice, bool wantToGoBack) = GetChoiceInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				var userRole = (choice == ChoiceConstants.ONE) ? UserRole.Admin : UserRole.Customer;

				Console.WriteLine("Enter Name:");
				(var name, wantToGoBack) = GetNameInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				Console.WriteLine("Enter Email:");
				(var email, wantToGoBack) = GetEmailInputForSignUpAndChangeEmail();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				Console.WriteLine("Enter Password:");
				(var password, wantToGoBack) = GetPasswordInput();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				User user = UserFactory.CreateUser(name, email, password, userRole);

				_userService.AddUser(user);
				_userService.Save();
				Console.WriteLine($"\n{userRole} account created successfully.");
				Animation.Loader.PressAnyKeyToExit();
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		public (User?, bool) Login()
		{
			try
			{
				Console.WriteLine("Enter Email for login:");
				(var email, bool wantToGoBack) = GetEmailInputForLogin();
				if (wantToGoBack)
				{
					return (null, true);
				}

				Console.WriteLine("Enter Password:");
				(var password, wantToGoBack) = GetPasswordInputForLogin();
				if (wantToGoBack)
				{
					return (null, true);
				}

				var users = _userService.GetAllUsers();
				var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);

				if (user == null)
				{
					return (null, false);
				}

				Console.WriteLine($"\nLogged in as {user.Role}");
				Animation.Loader.PressAnyKeyToExit();
				return (user, false);
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
				return (null, false);
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

				(var input, bool wantToGoBack) = GetChoiceInputForEditProfile();
				if (wantToGoBack)
				{
					Console.Clear();
					return;
				}

				if (input == ChoiceConstants.ONE)
				{
					Console.WriteLine("Enter new name");
					(var name, wantToGoBack) = GetNameInput();
					if (wantToGoBack)
					{
						Console.Clear();
						return;
					}

					user.Name = name;
					_userService.Save();
					Console.WriteLine("Name changed successfully");
					Animation.Loader.PressAnyKeyToExit();
					return;
				}
				if (input == ChoiceConstants.TWO)
				{
					Console.WriteLine("Enter new Email");
					(var email, wantToGoBack) = GetEmailInputForSignUpAndChangeEmail();
					if (wantToGoBack)
					{
						Console.Clear();
						return;
					}

					user.Email = email;
					_userService.Save();
					Console.WriteLine("Email id changed successfully.");
					Animation.Loader.PressAnyKeyToExit();
					return;
				}
				if (input == ChoiceConstants.THREE)
				{
					Console.WriteLine("Enter new Password:");
					(var newPassword, wantToGoBack) = GetPasswordInput();
					if (wantToGoBack)
					{
						Console.Clear();
						return;
					}

					Console.WriteLine("\nEnter old Password:");
					(var password, wantToGoBack) = GetPasswordInput();
					if (wantToGoBack)
					{
						Console.Clear();
						return;
					}

					if (password == user.Password)
					{
						user.Password = newPassword;
						_userService.Save();
						Console.WriteLine("\nPassword changed successfully");
					}
					else
					{
						Console.WriteLine("\nOld password did not match.");
					}
					Animation.Loader.PressAnyKeyToExit();
					return;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong.");
				CustomLogger.Logger.LogError(e);
			}
		}

		private (string, bool) GetChoiceInput()
		{
			while (true)
			{
				var input = Console.ReadLine();
				if (input == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (input, true);
				}
				if (string.IsNullOrWhiteSpace(input))
				{
					Console.WriteLine("Input cannot be empty.");
					continue;
				}
				if (input == ChoiceConstants.ONE || input == ChoiceConstants.TWO)
				{
					return (input, false);
				}
				Console.WriteLine("Enter valid choice.");
			}
		}

		private (string, bool) GetNameInput()
		{
			while (true)
			{
				var name = Console.ReadLine();
				if (name == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (name, true);
				}

				if (string.IsNullOrWhiteSpace(name))
				{
					Console.WriteLine("Name cannot be empty. Please try again.");
					continue;
				}
				return (name, false);
			}
		}

		private (string, bool) GetEmailInputForSignUpAndChangeEmail()
		{
			while (true)
			{
				var email = Console.ReadLine();
				if (email == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (email, true);
				}
				if (string.IsNullOrWhiteSpace(email))
				{
					Console.WriteLine("Email cannot be empty. Please try again.");
					continue;
				}
				if (!email.IsValidEmail())
				{
					Console.WriteLine("Please provide valid email.");
					continue;
				}
				if (_userService.GetAllUsers().FirstOrDefault(u => u.Email == email) != null)
				{
					Console.WriteLine("Email Id Exists. Try with another email Id");
					continue;
				}
				return (email, false);
			}
		}

		private (string, bool) GetPasswordInput()
		{
			while (true)
			{
				var password = Utils.PasswordReader.ReadPassword();
				if (password == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (password, true);
				}
				if (string.IsNullOrWhiteSpace(password))
				{
					Console.WriteLine("Password cannot be empty. Please try again.");
					continue;
				}
				if (!password.IsValidPassword())
				{
					Console.WriteLine("\nPassword length must be atleast 8 with 1 Uppercase character, 1 symbol and a numeric character");
					continue;
				}
				return (password, false);
			}
		}

		private (string, bool) GetPasswordInputForLogin()
		{
			while (true)
			{
				var password = Utils.PasswordReader.ReadPassword();
				if (password == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (password, true);
				}
				if (string.IsNullOrWhiteSpace(password))
				{
					Console.WriteLine("Password cannot be empty. Please try again.");
					continue;
				}
				return (password, false);
			}
		}

		private (string, bool) GetChoiceInputForEditProfile()
		{
			while (true)
			{
				var input = Console.ReadLine();
				if (input == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (input, true);
				}
				if (string.IsNullOrWhiteSpace(input) || !(input == ChoiceConstants.ONE || input == ChoiceConstants.TWO || input == ChoiceConstants.THREE))
				{
					Console.WriteLine("Enter valid choice");
					continue;
				}
				return (input, false);
			}
		}

		private (string, bool) GetEmailInputForLogin()
		{
			while (true)
			{
				var email = Console.ReadLine();
				if (email == AppConstants.Constants.BACK)
				{
					Console.Clear();
					return (email, true);
				}
				if (string.IsNullOrWhiteSpace(email))
				{
					Console.WriteLine("Email cannot be empty. Please try again.");
					continue;
				}
				if (!email.IsValidEmail())
				{
					Console.WriteLine("Please provide valid email.");
					continue;
				}
				return (email, false);
			}
		}
	}
}
