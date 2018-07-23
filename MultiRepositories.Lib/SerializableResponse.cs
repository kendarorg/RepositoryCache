using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace MultiRepositories
{

    public class SerializableResponse
    {
        public SerializableResponse()
        {
            HttpCode = 200;
            Headers = new Dictionary<string, string>();
            Content = new byte[] { };
        }
        public SerializableResponse Clone()
        {
            var res = new SerializableResponse
            {
                HttpCode = HttpCode,
                ContentType = ContentType
            };
            foreach (var item in Headers)
            {
                res.Headers.Add(item.Key, item.Value);
            }
            res.Content = Content;
            return res;
        }
        public int HttpCode { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public byte[] Content { get; set; }
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