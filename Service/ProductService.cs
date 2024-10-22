using ECommerce.Models;
using ECommerce.Repositories;

namespace ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void AddProduct(Product product)
        {
            try
            {
                _productRepository.Add(product);
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
            }
        }

        public Product GetProductById(Guid id)
        {
            try
            {
                return _productRepository.GetById(id);
            }catch(Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
                throw e;
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                return _productRepository.GetAll();
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong.");
                CustomLogger.Logger.LogError(e);
                throw e;
            }
        }
    }
}
