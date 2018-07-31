using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Services
{
    public class InsertMavenService : IInsertMavenService
    {
        public void Insert(Guid repoId, string login, string password, byte[] content)
        {
            var commitTimestamp = DateTime.UtcNow;

            var data = new InsertData
            {
                CommitId = Guid.NewGuid(),
                Nuspec = Deserialize(content, out commitTimestamp),
                Size = content.Length,
                RepoId = repoId,
            };
        }

        private object Deserialize(object content, out DateTime commitTimestamp)
        {
            throw new NotImplementedException("TODO ");
        }
    }
}
