using Ioc;

namespace Maven.Services
{
    public interface IQueryBuilder : ISingleton
    {
        ParsedQuery ParseQuery(string q);
    }
}