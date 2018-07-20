using Ioc;

namespace Nuget.Services
{
    public interface IQueryBuilder : ISingleton
    {
        ParsedQuery ParseQuery(string q);
    }
}