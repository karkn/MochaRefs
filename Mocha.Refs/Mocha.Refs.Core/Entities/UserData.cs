using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class UserData
    {
        public virtual long UserId { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual User User { get; set; }
    }
}
