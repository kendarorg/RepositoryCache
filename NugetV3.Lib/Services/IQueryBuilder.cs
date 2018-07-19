namespace NugetV3.Services
{
    public interface IQueryBuilder
    {
        ParsedQuery ParseQuery(string q);
    }
}