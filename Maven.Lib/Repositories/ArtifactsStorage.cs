using Ioc;
using MavenProtocol;
using MavenProtocol.Apis;
using MultiRepositories.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Maven.Services
{
    public class ArtifactsStorage : IArtifactsStorage, ISingleton
    {
        private readonly IServicesMapper _servicesMapper;

        public ArtifactsStorage(IServicesMapper servicesMapper)
        {
            this._servicesMapper = servicesMapper;
        }
        public List<string> GetSubDir(RepositoryEntity repo, MavenIndex index)
        {
            var path = GetPath(repo);
            var group = string.Join("\\", index.Group);
            if(index.Group!=null && index.Group.Any())
            {
                if (group != "")
                {
                    path = path + "\\" + group;
                }
                if (!string.IsNullOrWhiteSpace(index.ArtifactId))
                {
                    path = path + "\\" + index.ArtifactId;
                    if (!string.IsNullOrWhiteSpace(index.Version))
                    {
                        return new List<string>();
                    }
                }
            }
            if (Directory.Exists(path))
            {
                return Directory.GetDirectories(path).Select(a => Path.GetFileName(a)).ToList();
            }
            return new List<string>();
        }

        public byte[] Load(RepositoryEntity repo, MavenIndex index)
        {
            
            var path = Path.Combine(GetPath(repo), index.ToLocalPath(_servicesMapper.HasTimestampedSnapshot(repo.Id)));
            return File.ReadAllBytes(path);
        }

        public void Save(RepositoryEntity repo, MavenIndex index, byte[] data)
        {
            var path = Path.Combine(GetPath(repo), index.ToLocalPath(_servicesMapper.HasTimestampedSnapshot(repo.Id)));
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
