using Ioc;
using MultiRepositories.Repositories;
using Nuget.Repositories;
using System.IO;

namespace Nuget.Services
{
    public class PackagesStorage : IPackagesStorage, ISingleton
    {
        public byte[] Load(RepositoryEntity repo, string id, string normalVersion)
        {
            var path = Path.Combine(GetPath(repo), id, id + "." + normalVersion + "nupkg");
            return File.ReadAllBytes(path);
        }

        public void Save(RepositoryEntity repo, string id, string normalVersion, byte[] data)
        {
            var path = Path.Combine(GetPath(repo), id, id + "." + normalVersion + "nupkg");
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
