using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nuget.Lib.Test.Utils;
using System.Net;
using System.IO;

namespace NugetProtocol.Test
{
    [TestClass]
    public class ShouldCorrespondWithApi
    {
        [TestMethod]
        public void ItShouldCompyWithNugetProtocol()
        {
            try
            {
                string googleData = CallUrl(@"https://www.google.com");
                Assert.IsNotNull(googleData);              
            }
            catch (WebException)
            {
                return;
            }

            string data = CallUrl(@"https://api.nuget.org/v3/index.json");
            Assert.IsTrue(JsonComp.EqualsString("apinugetorg.v3.index.json", data));
            
        }

        private static string CallUrl(string url)
        {
            string html = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 200;
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }
    }
}
