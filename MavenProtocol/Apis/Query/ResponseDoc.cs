using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MavenProtocol.Apis
{
    public class ResponseDoc
    {
        public ResponseDoc(string id,string group,string archive,string version,
            long timestamp,
            List<string> typeAndExt,List<string> tags)
        {
            Id = id;
            Group = group;
            Archive = archive;
            Version = version;
            Timestamp = timestamp;
            TypeAndExt = typeAndExt;
            Tags = tags;
        }

        /// <summary>
        /// Package:Name:Version
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("g")]
        public string Group { get; set; }
        [JsonProperty("a")]
        public string Archive { get; set; }
        [JsonProperty("v")]
        public string Version { get; set; }
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// given package test-1.2.jar
        /// Returns
        ///     test-1.2[.jar]
        ///     test-1.2[-sources.jar]
        ///     test-1.2[.pom]
        /// Does not return sha,md5 and asc
        /// </summary>
        [JsonProperty("ec")]
        public List<string> TypeAndExt { get; }
        [JsonProperty("tags")]
        public List<string> Tags { get; }
    }
}