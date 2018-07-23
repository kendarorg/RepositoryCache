using MultiRepositories.Repositories;
using Nuget.Repositories;
using System.IO;

namespace Nuget.Services
{
    public class PackagesStorage : IPackagesStorage
    {
        public byte[] Load(RepositoryEntity repo, PackageEntity package)
        {
            var path = Path.Combine(GetPath(repo), package.PackageId, package.PackageId + "." + package.Version + "nupkg");
            return File.ReadAllBytes(path);
        }

        public void Save(RepositoryEntity repo, PackageEntity package, byte[] data)
        {
            var path = Path.Combine(GetPath(repo), package.PackageId, package.PackageId + "." + package.Version + "nupkg");
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
