﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories.Service
{
    public class FileRestAPI : RestAPI
    {
        public FileRestAPI(params string[] paths) : base((a)=> { return null; }, paths)
        {
        }
    }
}
