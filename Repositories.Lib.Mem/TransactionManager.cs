using Ioc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TransactionManager : ITransactionManager, ISingleton
    {
        public ITransaction BeginTransaction()
        {
            return new Transaction();
        }
    }
}
