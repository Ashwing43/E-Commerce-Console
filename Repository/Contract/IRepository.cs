using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IRepository<T>
    {
        void Add(T item);
        T GetById(Guid id);
        IEnumerable<T> GetAll();
    }
}