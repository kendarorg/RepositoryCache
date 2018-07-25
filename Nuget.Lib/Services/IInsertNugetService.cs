using Ioc;
using NugetProtocol;
using Repositories;
using System;

namespace Nuget.Services
{
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