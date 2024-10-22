using ECommerce.Models;
using ECommerce.Repositories;

namespace ECommerce.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
                _userRepository = userRepository;
        }

        public void AddUser(User user)
        {
            try
            {
                _userRepository.Add(user);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public User GetUserById(Guid id)
        {
            try
            {
                return _userRepository.GetById(id);
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
                throw e;
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            try
            {
                return _userRepository.GetAll();
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
                User user = _userRepository.GetById(id);
                user.DisplayInfo();
                Console.WriteLine("\nPress any key to return to menu.");
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
