using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol
{
    public static class MavenConstants
    {
        public const string PACKAGE_REGEXP =
                    @"^(?<package>[0-9A-Za-z\-\.]+)$";
        public const string VERSION_REGEXP = 
                    @"^(?<major>\d+)" + //1.7.25/
                    @"(\.(?<minor>\d+))?" +
                    @"(\.(?<patch>\d+))?" +
                    @"(\.(?<extra>\d+))?" +
                    @"(\-(?<pre>[0-9A-Za-z\.]+))?" +
                    @"(\-(?<build>[0-9A-Za-z\-\.]+))?$";
        public const string FULLPACKAGE_AND_CHECHKSUMS_REGEXP =
                    @"^(?<filename>(?:(?!\b(?:jar|pom)\b)[0-9A-Za-z\-\.])+)?" +
                    @"\.(?<type>(jar|pom))" +
                    @"(\.(?<subtype>(asc|md5|sha1)))?$";
        public const string METADATA_AND_CHECHKSUMS_REGEXP =
                    @"^(?<filename>(maven-metadata))"+
                    @"\.(?<type>(xml))" +
                    @"(\.(?<subtype>(asc|md5|sha1)))?$";


    }
}
