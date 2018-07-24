using Ioc;
using NugetProtocol;
using System;

namespace Nuget.Services
{
    public interface IInsertNugetService: ISingleton
    {
        void Insert(Guid repoId, string nugetApiKey, byte[] data);
        void InsertQuery(InsertData data);
        void InsertRegistration(InsertData data);
        void InsertPackages(InsertData data);
        void InsertPackagesStorage(InsertData data,byte[] content);
        void InsertDependencies(InsertData data);
        void InsertAssemblies(InsertData data);
    }
}