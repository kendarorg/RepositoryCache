using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultiRepositories.Repositories
{
    public class UserEntity : BaseEntity
    {
        public String Login { get; set; }
        public String Password { get; set; }
        public DateTime LastAccess { get; set; }

        public UserEntity CleanClone()
        {
            return new UserEntity
            {
                Id = Id,
                Login = Login,
                Password = string.Empty,
                LastAccess = LastAccess
            };
        }
    }
}