using Ioc;
using MultiRepositories.Repositories;
using System.IO;
using MavenProtocol.Apis;

namespace Maven.Services
{
    public class PackagesStorage //: IPackagesStorage, ISingleton
    {
#if NOT
        public byte[] ReadPackage(RepositoryEntity repo, MavenVersionedArtifact item)
        {
            var path = Path.Combine(GetPath(repo), item.ToLocalPath());
            return File.ReadAllBytes(path);
        }

        public string ReadPom(RepositoryEntity repo, MavenVersionedArtifact item)
        {
            throw new System.NotImplementedException();
        }

        /*public byte[] Load(RepositoryEntity repo, string id, string normalVersion)
{
   var path = Path.Combine(GetPath(repo), id, id + "." + normalVersion + ".nupkg");
   return File.ReadAllBytes(path);
}

public void Save(RepositoryEntity repo, string id, string normalVersion, byte[] data)
{
   var path = Path.Combine(GetPath(repo), id, id + "." + normalVersion + ".nupkg");
   var dir = Path.GetDirectoryName(path);
   if (!Directory.Exists(dir))
   {
       Directory.CreateDirectory(dir);
   }
   File.WriteAllBytes(path, data);
}*/

        private string GetPath(RepositoryEntity repo)
        {
            if (string.IsNullOrWhiteSpace(repo.PackagesPath))
            {
                return repo.Id.ToString();

            }
            return repo.PackagesPath;
        }
#endif
    }
}
