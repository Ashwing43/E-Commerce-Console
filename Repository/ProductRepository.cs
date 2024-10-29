using ECommerce.Models;
using DataStorage;

namespace ECommerce.Repositories
{
	public class ProductRepository : IProductRepository
	{
		public void Add(Product product)
		{
			DataStore.Products.Add(product);
			DataStore.SaveData();
		}

		public Product? GetById(Guid id)
		{
			return DataStore.Products.FirstOrDefault(p => p.Id == id);
		}

		public IEnumerable<Product> GetAll()
		{
			return DataStore.Products;
		}
		public void Save()
		{
			DataStore.SaveData();
		}
	}
}
