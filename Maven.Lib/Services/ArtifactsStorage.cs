using Ioc;
using MultiRepositories.Repositories;
using System.IO;
using MavenProtocol.Apis;
using System.Collections.Generic;
using System.Linq;

namespace Maven.Services
{
    public class ArtifactsStorage : IArtifactsStorage
    {
        public List<string> ListChildren(RepositoryEntity repo, string[] group)
        {
            var path = Path.Combine(GetPath(repo), string.Join("\\", group));
            var result = new List<string>();


            if (!Directory.Exists(path)) return result;

            result.AddRange(Directory.GetFiles(path).Select(a => Path.GetFileName(a)));
            result.AddRange(Directory.GetDirectories(path).Select(a => Path.GetFileName(a)));
            return result;
        }

        public void Write(RepositoryEntity repo, string[] group, string artifactId, string version, string classifier, string type, byte[] data)
        {
            classifier = string.IsNullOrWhiteSpace(classifier) ? "" : "-" + classifier;
            var path = Path.Combine(GetPath(repo), string.Join("\\", group),
                artifactId, version, artifactId + "-" + version + classifier + "." + type);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(path, data);
        }

        public byte[] Load(RepositoryEntity repo, string[] group, string artifactId, string version, string classifier, string type)
        {
            classifier = string.IsNullOrWhiteSpace(classifier) ? "" : "-" + classifier;
            var path = Path.Combine(GetPath(repo), string.Join("\\", group),
                artifactId, version, artifactId + "-" + version + classifier + "." + type);
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            return null;
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
