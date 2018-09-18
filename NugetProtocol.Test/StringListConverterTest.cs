using System;
using NUnit.Framework;
using Ioc;
using Newtonsoft.Json;
using Nuget.Lib.Test.Utils;

namespace NugetProtocol.Test
{
    [TestFixture]
    public class StringListConverterTest
    {
        [Test]
        public void ShouldConvertStringAndArrays()
        {//[JsonConverter(typeof(StringListConverter))]
            var au = new AssemblyUtils();
            var expected = au.ReadRes<StringListConverterTest>("registryentry.comp.json");
            var json = au.ReadRes<StringListConverterTest>("registryentry.json");
            var serialized = JsonConvert.DeserializeObject<PackageDetail>(json);
            var deserialized = JsonConvert.SerializeObject(serialized);
            
            JsonComp.EqualsSimple(expected, deserialized);
        }

        //https://api.nuget.org/v3/catalog0/data/2018.05.02.13.34.24/ravendb.database.1.0.728-unstable.json
        [Test]
        public void ShouldConvertObjecctAndArrays()
        {//[JsonConverter(typeof(StringListConverter))]
            var au = new AssemblyUtils();
            var expected =  au.ReadRes<StringListConverterTest>("catalogentry.comp.json");
            var json = au.ReadRes<StringListConverterTest>("catalogentry.json");
            var serialized = JsonConvert.DeserializeObject<CatalogEntry>(json);
            var deserialized = JsonConvert.SerializeObject(serialized);

            JsonComp.EqualsSimple(expected, deserialized);
        }
    }
}
