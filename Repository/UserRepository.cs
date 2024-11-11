using DataStorage;
using ECommerce.Models;

namespace ECommerce.Repositories
{
	public class UserRepository : IUserRepository
	{
		public void Add(User user)
		{
			DataStore.Users.Add(user);
			DataStore.SaveData();
		}

		public User GetById(Guid id)
		{
			return DataStore.Users.FirstOrDefault(u => u.Id == id);
		}

		public IEnumerable<User> GetAll()
		{
			return DataStore.Users;
		}
		public void Save()
		{
			DataStore.SaveData();
		}
	}
}
