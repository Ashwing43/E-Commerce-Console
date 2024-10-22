using ECommerce.Models;
using Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        void Add(Product product);
        Product GetById(Guid id);
        IEnumerable<Product> GetAll();
    }
}
