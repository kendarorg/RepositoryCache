using System;

namespace Repositories
{
    public class Transaction : ITransaction
    {
        public void Commit()
        {
            
        }

        public void Dispose()
        {
            
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
