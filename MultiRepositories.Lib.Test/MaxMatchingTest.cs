using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultiRepositories
{
    [TestClass]
    public class MaxMatchingTest
    {
        public const string REGEX_ONLY_PACK = @"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/" +
            @"(?<package>[0-9A-Za-z\-\.]+)/(?<version>[0-9A-Za-z\-\.]+)/\3-\4" +
            @"(\-(?<specifier>[0-9A-Za-z\-]+))?(\.(?<extension>[0-9A-Za-z]+))(\.(?<checksum>(asc|md5|sha1)))?$";

        public const string REGEX_ONLY_META = @"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/" +
            @"(?<package>[0-9A-Za-z\-\.]+)/(?<meta>maven\-metadata\.xml)(\.(?<checksum>(asc|md5|sha1)))?$";

        public const string VERSION_SPLIT_REGEXP =
                    @"^(?<major>\d+)" + //1.7.25/
                    @"(\.(?<minor>\d+))?" +
                    @"(\.(?<patch>\d+))?" +
                    @"(\.(?<extra>\d+))?" +
                    @"(\-(?<pre>[0-9A-Za-z\-\.]+))?$";

        public const string PACKAGE_REGEXP =
                    @"^(?<package>[0-9A-Za-z\-\.]+)$";
        public const string VERSION_REGEXP =
                    @"^(?<major>\d+)" + //1.7.25/
                    @"(\.(?<minor>\d+))?" +
                    @"(\.(?<patch>\d+))?" +
                    @"(\.(?<extra>\d+))?" +
                    @"(\-(?<pre>[0-9A-Za-z\-\.]+))?$";
        public const string FULLPACKAGE_AND_CHECHKSUMS_REGEXP =
                    @"^(?<filename>(?:(?!\b(?:jar|pom)\b)[0-9A-Za-z\-\.])+)?" +
                    @"\.(?<type>(jar|pom))" +
                    @"(\.(?<subtype>(asc|md5|sha1)))?$";
        public const string METADATA_AND_CHECHKSUMS_REGEXP =
                    @"^(?<type>(maven-metadata))" +
                    @"\.(?<extension>(xml))" +
                    @"(\.(?<subtype>(asc|md5|sha1)))?$";


        [TestMethod]
        public void ISBPToMatchRegexJarMd5Comp()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            },
                        "maven.local",

                        REGEX_ONLY_META.
                            Replace("{repo}", Regex.Escape("maven.local")),

                        @"/{repo}/{*path}/" + ///maven.local/org/slf4j
                        @"{pack#" + PACKAGE_REGEXP + @"}/" + //slf4j-api/
                        @"{version#" + VERSION_REGEXP + @"}". //1.7.2
                            Replace("{repo}", "maven.local"),


                         REGEX_ONLY_PACK.
                             Replace("{repo}", Regex.Escape("maven.local")),



                         @"/{repo}/{*path}/" +//maven.local/org/slf4j/
                         @"{pack#" + PACKAGE_REGEXP + @"}".//slf4j-api/
                             Replace("{repo}", "maven.local"),

                        @"/{repo}/{*path}".
                            Replace("{repo}", "maven.local")//maven.local/org/slf4j/
                            );

            var url = "/maven.local/org/slf4j/slf4j-api/a1.7.25/slf4j-api-1.7.25.jar.md5";
            Assert.IsFalse(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest() { Url = url };
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);
        }
    }
}
