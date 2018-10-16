using Ioc;
using NugetProtocol;
using Repositories;
using System;
using System.Collections.Generic;

namespace Nuget.Services
{
    public class DeserializedPackage
    {
        public DeserializedPackage()
        {
            Frameworks = new List<string>();
        }
        public PackageXml Nuspec { get; set; }
        public List<string> Frameworks { get; set; }
    }
    public interface IInsertNugetService: ISingleton
    {
        void Insert(Guid repoId, string nugetApiKey, byte[] data);
        void InsertQuery(InsertData data, ITransaction transaction);
        void InsertRegistration(InsertData data, ITransaction transaction);
        void InsertPackages(InsertData data, ITransaction transaction);
        void InsertPackagesStorage(InsertData data,byte[] content);
        void InsertDependencies(InsertData data, ITransaction transaction);
        void InsertAssemblies(InsertData data, ITransaction transaction);
    }
}