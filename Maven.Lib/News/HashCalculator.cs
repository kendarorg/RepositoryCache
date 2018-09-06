using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public class HashCalculator : IHashCalculator
    {
        public string GetMd5(string data)
        {
            return GetMd5(System.Text.Encoding.ASCII.GetBytes(data));
        }

        public string GetMd5(byte[] inputBytes)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string GetSha1(string data)
        {
            return GetSha1(System.Text.Encoding.ASCII.GetBytes(data));
        }

        public string GetSha1(byte[] inputBytes)
        {
            SHA1 md5 = System.Security.Cryptography.SHA1.Create();
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
