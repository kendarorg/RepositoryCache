using Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiRepositories;
using Newtonsoft.Json;
using Nuget.Lib.Test.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Lib.Test.Controllers
{
    public class BaseControllersTest
    {
        private AssemblyUtils _assemblyUtils = new AssemblyUtils();

        protected bool AreEquals<T>(string fileName, SerializableResponse result)
        {
            
            Assert.IsNotNull(result);
            var response = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(result.Content));
            return JsonComp.Equals("ISPToQuery" + ".data.json", response);
        }

        protected SerializableResponse HandleRequest(string file, string realUrl, SerializableRequest req)
        {
            JsonComp.Equals(file + ".req.json", req);
            return JsonConvert.DeserializeObject<SerializableResponse>(
                _assemblyUtils.ReadRes<NugetServicesMapperTest>(file + ".res.json"));

        }
    }
}
