using Repositories;

namespace MultiRepositories.Repositories
{
    public class GroupEntity : BaseEntity
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string AdminIds { get; set; }
        public string UserIds { get; set; }
    }
}