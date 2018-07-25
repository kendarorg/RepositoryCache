using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public class StringListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, 
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                if (reader.Value == null || string.IsNullOrWhiteSpace(reader.Value.ToString()))
                {
                    return null;
                }

                return new List<string> { reader.Value.ToString() };
            }
            var t = JToken.ReadFrom(reader);
            JArray o = (JArray)t;
            List<string> vals = o.Values().Select(a => a.ToString()).ToList();
            if (vals.Any())
            {
                return vals;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            JArray o = (JArray)t;
            o.WriteTo(writer);

        }
    }
}
