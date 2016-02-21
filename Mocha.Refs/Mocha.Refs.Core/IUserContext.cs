using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core
{
    public interface IUserContext
    {
        bool IsAuthenticated { get;}
        User GetUser();
    }
}
