using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class RegistrationPage
    {
        public RegistrationPage()
        {

        }
        public RegistrationPage(
            string oid, string otype,
            Guid commitId, DateTime commitTimestamp, int count,
            string lower, string upper,
            string parent,
            List<RegistrationLeaf> items,
            RegistrationContext context)
        {
            OId = oid;
            OType = otype;
            CommitId = commitId;
            CommitTimestamp = commitTimestamp;
            Count = count;
            Lower = lower;
            Upper = upper;
            Parent = parent;
            Items = items;
            OContext = context;
        }

        public string OId { get; set; }
        public string OType { get; set; }
        public Guid CommitId { get; set; }
        public DateTime CommitTimestamp { get; set; }
        public int Count { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }
        public string Parent { get; set; }
        public List<RegistrationLeaf> Items { get; set; }
        public RegistrationContext OContext { get; set; }
    }
}
