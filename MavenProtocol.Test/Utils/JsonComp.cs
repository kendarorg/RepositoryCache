using Ioc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Lib.Test.Utils
{
    public class JsonComp
    {
        private static AssemblyUtils _au = new AssemblyUtils();
        public static bool Equals(string resIdExpected, Object result)
        {
            return EqualsString(resIdExpected, JsonConvert.SerializeObject(result));
        }

        public static bool EqualsString(string resIdExpected, string result)
        {
            var founded = Beautify(result).Trim();
            var expected = Beautify(_au.ReadRes<JsonComp>(resIdExpected)).Trim();

            var foundedSplitted = founded.Split('\r', '\n', '\f').Where(r => r.Trim().Length > 0).ToArray();
            var expectedSplitted = expected.Split('\r', '\n', '\f').Where(r => r.Trim().Length > 0).ToArray();

            if (foundedSplitted.Length != expectedSplitted.Length)
            {
                throw new Exception(string.Format("Expected Length {0}\r\nReal Length {1}", expectedSplitted.Length, foundedSplitted.Length));
            }
            for (int i = 0; i < expectedSplitted.Length; i++)
            {
                if (expectedSplitted[i] != foundedSplitted[i])
                {
                    throw new Exception(string.Format("Expected {0}\r\nReal {1}\r\nLine {2}", expectedSplitted[i], foundedSplitted[i], i));
                }
            }
            return true;
        }

        public static string Beautify(string tob)
        {
            try
            {
                JToken parsedJson = JToken.Parse(tob);
                return parsedJson.ToString(Formatting.Indented);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
