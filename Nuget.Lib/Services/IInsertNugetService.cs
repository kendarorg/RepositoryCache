using Ioc;
using System;

namespace Nuget.Services
{
    public interface IInsertNugetService: ISingleton
    {
        void Insert(Guid repoId, string nugetApiKey, byte[] data);
    }
}