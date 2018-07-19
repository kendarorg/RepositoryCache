
using Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IRepository: ISingleton
    {
        void Initialize();
    }
    public interface IRepository<T>: IRepository where T : BaseEntity, new()
    {
        void Clean(ITransaction transaction=null);
        int Save(T be, ITransaction transaction = null);
        int Update(T be, ITransaction transaction = null);
        T GetById(Guid id, ITransaction transaction = null);
        long Count { get; }
        void Delete(Guid id, ITransaction transaction = null);
        IEnumerable<T> GetAll(ITransaction transaction = null);
    }
}
