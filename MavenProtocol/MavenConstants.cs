using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol
{
    public static class MavenConstants
    {

        /**
         * <extension>((?!(sha1|md5|asc))[0-9A-Za-z\.]+) match not containing in this occurrence the sha1|md|asc
         *  <extension>((?!.*(sha1|md5|asc))[0-9A-Za-z\.]+) match not containing any of them in the following part
         */
        /*"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/(?<package>[0-9A-Za-z\-\.]+)/(?<version>((?!SNAPSHOT)[0-9A-Za-z\-\.])+)-SNAPSHOT/\3-\4-(?<build>[0-9]{8}\.[0-9]{6}\-[0-9]+)(\-(?<specifier>[0-9A-Za-z\-]+))?(\.(?<extension>((?!(sha1|md5|asc))[0-9A-Za-z\.]+)))$"
        "repo/org/kendar/test/1.2-SNAPSHOT/test-1.2-12345678.123456-1-gioppino.tar.gz"
        "repo/org/kendar/test/1.2-SNAPSHOT/test-1.2-12345678.123456-1-gioppino.jar"

        "^(?<repoId>repo)/(?<path>[0-9A-Za-z\-\./]+)/(?<package>[0-9A-Za-z\-\.]+)/(?<version>((?!SNAPSHOT)[0-9A-Za-z\-\.])+)-SNAPSHOT/\3-\4-(?<build>[0-9]{8}\.[0-9]{6}\-[0-9]+)(\-(?<specifier>[0-9A-Za-z\-]+))?(\.(?<extension>((?!(sha1|md5|asc))[0-9A-Za-z\.]+)))(\.(?<checksum>(asc|md5|sha1)))$"
        "repo/org/kendar/test/1.2-SNAPSHOT/test-1.2-12345678.123456-1-gioppino.jar.md5"
        "repo/org/kendar/test/1.2-SNAPSHOT/test-1.2-12345678.123456-1-gioppino.tar.gz.md5"*/


        /*public const string REGEX_ONLY_PACK = @"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/" +
            @"(?<package>[0-9A-Za-z\-\.]+)/(?<version>[0-9A-Za-z\-\.]+)/\3-\4" +
            @"(\-(?<specifier>[0-9A-Za-z\-]+))?(\.(?<extension>[0-9A-Za-z]+))(\.(?<checksum>(asc|md5|sha1)))?$";*/

        public const string REGEX_ONLY_PACK = @"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/" +
            @"(?<package>[0-9A-Za-z\-\.]+)/(?<version>[0-9A-Za-z\-\.]+)/\3-\4" +
            @"(\-(?<specifier>[0-9A-Za-z\-]+))?(\.(?<extension>[0-9A-Za-z\.]+))$";

        public const string REGEX_ONLY_META = @"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/" +
            @"(?<package>[0-9A-Za-z\-\.]+)/(?<meta>maven\-metadata\.xml)(\.(?<checksum>(asc|md5|sha1)))?$";


        public const string REGEX_SNAP_PACK = @"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/(?<package>[0-9A-Za-z\-\.]+)/(?<version>((?!SNAPSHOT)[0-9A-Za-z\-\.])+)(?<snapshot>(\-SNAPSHOT))/\3-\4-(?<build>[0-9]{8}\.[0-9]{6}\-[0-9]+)(\-(?<specifier>[0-9A-Za-z\-]+))?(\.(?<extension>((?!(sha1|md5|asc))[0-9A-Za-z\.]+)))$";
        public const string REGEX_SNAP_PACK_CHECK = @"^(?<repoId>repo)/(?<path>[0-9A-Za-z\-\./]+)/(?<package>[0-9A-Za-z\-\.]+)/(?<version>((?!SNAPSHOT)[0-9A-Za-z\-\.])+)(?<snapshot>(\-SNAPSHOT))/\3-\4-(?<build>[0-9]{8}\.[0-9]{6}\-[0-9]+)(\-(?<specifier>[0-9A-Za-z\-]+))?(\.(?<extension>((?!(sha1|md5|asc))[0-9A-Za-z\.]+)))(\.(?<checksum>(asc|md5|sha1)))$";
        public const string REGEX_SNAP_META = @"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/(?<package>[0-9A-Za-z\-\.]+)/(?<version>((?!SNAPSHOT)[0-9A-Za-z\-\.])+)(?<snapshot>(\-SNAPSHOT))/(?<meta>maven\-metadata\.xml)(\.(?<checksum>(asc|md5|sha1)))?$";


        /*public const string REGEX_SUB_META = @"^(?<repoId>{repo})/(?<path>[0-9A-Za-z\-\./]+)/" +
        @"(?<package>[0-9A-Za-z\-\.]+)/(?<version>[0-9A-Za-z\-\.]+)/(?<meta>maven\-metadata\.xml)(\.(?<checksum>(asc|md5|sha1)))?$";*/

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
                    @"^(?<type>(maven-metadata))"+
                    @"\.(?<extension>(xml))" +
                    @"(\.(?<subtype>(asc|md5|sha1)))?$";


    }
}
