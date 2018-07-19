using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class RegistrationIndex
    {
        public RegistrationIndex()
        {
        }
        public RegistrationIndex(
            string oid, List<string> otype,
            Guid commitId, DateTime commitTimestamp,
            int count, List<RegistrationPage> items,
            RegistrationContext ocontext)
        {
            OId = oid;
            OType = otype;
            CommitId = commitId;
            CommitTimestamp = commitTimestamp;
            Count = count;
            Items = items;
            Ocontext = ocontext;
        }

        public string OId { get; set; }
        public List<string> OType { get; set; }
        public Guid CommitId { get; set; }
        public DateTime CommitTimestamp { get; set; }
        public int Count { get; set; }
        public List<RegistrationPage> Items { get; set; }
        public RegistrationContext Ocontext { get; set; }
    }
}
