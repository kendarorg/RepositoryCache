using Ioc;
using MavenProtocol.Apis;
using MultiRepositories.Repositories;
using System.IO;

namespace Nuget.Services
{
    public class ArtifactsStorage : IArtifactsStorage, ISingleton
    {
        public byte[] Load(RepositoryEntity repo, MavenIndex index)
        {
            
            var path = Path.Combine(GetPath(repo), index.ToLocalPath());
            return File.ReadAllBytes(path);
        }

        public void Save(RepositoryEntity repo, MavenIndex index, byte[] data)
        {
            var path = Path.Combine(GetPath(repo), index.ToLocalPath());
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(path, data);
        }

        private string GetPath(RepositoryEntity repo)
        {
            if (string.IsNullOrWhiteSpace(repo.PackagesPath))
            {
                return repo.Id.ToString();

            }
            return repo.PackagesPath;
        }
    }
}
