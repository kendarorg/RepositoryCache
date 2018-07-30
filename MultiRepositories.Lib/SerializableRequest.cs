using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MultiRepositories
{
    public class SerializableRequest
    {
        public SerializableRequest Clone()
        {
            var c = new SerializableRequest
            {
                Content = this.Content,
                ContentType = this.ContentType,
                Host = this.Host,
                Method = this.Method,
                Url = this.Url,
                Protocol = this.Protocol,
                Log = this.Log,
                RealUrl = this.RealUrl
            };
            CloneHash(c.Headers, Headers);
            CloneHash(c.PathParams, PathParams);
            CloneHash(c.QueryParams, QueryParams);
            return c;
        }

        public string ToLocalPath(String fileName = null)
        {
            var res = Url.Trim('/').Replace("/", "\\");

            if (QueryParams != null && QueryParams.Any())
            {
                res = res + "\\" + CalculateMD5Hash(JsonConvert.SerializeObject(QueryParams));
            }
            if (fileName == null) return res + "." + Method;
            return res + "\\" + Method + "." + fileName;
        }

        public string CalculateMD5Hash(string input)

        {

            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);


            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("X2"));

            }

            return sb.ToString();

        }

        private void CloneHash(Dictionary<string, string> headers1, Dictionary<string, string> headers2)
        {
            foreach (var h in headers2)
            {
                headers1[h.Key] = h.Value;
            }
        }

        public SerializableRequest()
        {
            Headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            PathParams = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            QueryParams = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            Content = new byte[] { };
            Protocol = "http";
        }
        public string Protocol { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> PathParams { get; set; }
        public byte[] Content { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
        public string Host { get; set; }
        public bool Log { get; set; }
        public string RealUrl { get; set; }

        public string Serialize()
        {

            var clone = Clone();
            clone.Content = null;
            var result = JsonConvert.SerializeObject(clone, Formatting.Indented);
            if (ContentType == "application/json")
            {
                result += "\n" + JValue.Parse(Encoding.UTF8.GetString(Content)).ToString(Formatting.Indented);
            }

            return result;
        }
    }
}