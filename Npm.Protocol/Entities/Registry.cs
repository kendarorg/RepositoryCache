using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Npm.Entities
{
    public class NpmTime :Dictionary<string,string>
    {
        [JsonProperty("modified", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Modified { get; set; }
        [JsonProperty("created", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Created { get; set; }
    }
    public class FullMetadata: AbbreviatedVersion
    {
        [JsonProperty("_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }
        [JsonProperty("_rev", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Rev { get; set; }
        [JsonProperty("time", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public NpmTime Time { get; set; }
    }
    public class AbbreviatedVersion
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("deprecated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Deprecated { get; set; }
        [JsonProperty("dependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string,string> Dependencies { get; set; }
        [JsonProperty("optionalDependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> OptionalDependencies { get; set; }
        [JsonProperty("devDependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> DevDependencies { get; set; }
        [JsonProperty("bundleDependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> BundleDependencies { get; set; }
        [JsonProperty("peerDependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> PeerDependencies { get; set; }
        [JsonProperty("bin", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> Bin { get; set; }
        [JsonProperty("directories", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> directories { get; set; }
        [JsonProperty("dist")]
        public Dist Dist { get; set; }
        [JsonProperty("engines", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> Engines { get; set; }
        [JsonProperty("_hasShrinkwrap", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool HasShrinkwrap { get; set; }
    }
    public class DistTags : Dictionary<string, string>
    {
        [JsonProperty("latest")]
        public string Latest { get; set; }
    }
    public class AbbreviatedMetadata
    {

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("modified")]  //ISO801
        public DateTime Modified { get; set; }
        [JsonProperty("dist-tags")]
        public DistTags DistTags { get; set; }
        [JsonProperty("versions")]
        public Dictionary<String,NpmVersion> Versions { get; set; }

    }
    public class Repository
    {

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
    public class Dist
    {

        [JsonProperty("tarball")]
        public string Tarball { get; set; }
        [JsonProperty("shasum")]
        public string Sha { get; set; }
    }
    public class Human
    {

        [JsonProperty("name")]
        public string DbName { get; set; }
        [JsonProperty("email")]
        public string EMail { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
    public class Registry
    {
        [JsonProperty("db_name")]
        public string DbName { get; set; }

        [JsonProperty("doc_count")]
        public long DocCount { get; set; }

        [JsonProperty("doc_del_count")]
        public long DocDelCount { get; set; }

        [JsonProperty("update_seq")]
        public long UpdateSeq { get; set; }

        [JsonProperty("purge_seq")]
        public long PurgeSeq { get; set; }

        [JsonProperty("compact_running")]
        public bool CompactRunning { get; set; }

        [JsonProperty("disk_size")]
        public long DiskSize { get; set; }

        [JsonProperty("data_size")]
        public long DataSize { get; set; }

        [JsonProperty("instance_start_time")]
        public string InstanceStartTime { get; set; }

        [JsonProperty("disk_format_version")]
        public long DiskFormatVersion { get; set; }

        [JsonProperty("committed_update_seq")]
        public long CommittedUpdateSeq { get; set; }
    }
}
