﻿using Ioc;
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
            var expected = Beautify(_au.ReadRes<JsonComp>(resIdExpected));
            var founded = Beautify(JsonConvert.SerializeObject(result));
            if(expected != founded)
            {
                throw new Exception(string.Format("Expected {0}\r\nReal {1}", expected, founded));
            }
            return true;
        }

        private static string Beautify(string tob)
        {
            JToken parsedJson = JToken.Parse(tob);
            return parsedJson.ToString(Formatting.Indented);
        }
    }
}