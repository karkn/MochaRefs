﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models.List
{
    [Serializable]
    public class IndexPageViewModel: PagedRefListsViewModel
    {
        public string Tag { get; set; }
    }
}