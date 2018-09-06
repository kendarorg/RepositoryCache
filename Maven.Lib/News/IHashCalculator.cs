using Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public interface IHashCalculator:ISingleton
    {
        string GetMd5(string data);
        string GetSha1(string data);
        string GetMd5(byte[] data);
        string GetSha1(byte[] data);
    }
}
