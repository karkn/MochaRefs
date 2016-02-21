﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class UserLogin
    {
        public virtual long UserId { get; set; }
        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }
        public virtual User User { get; set; }
    }
}
