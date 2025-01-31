using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natia.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task Add(T item);
        Task Remove(int id);
        Task View(int id);
    }
}
