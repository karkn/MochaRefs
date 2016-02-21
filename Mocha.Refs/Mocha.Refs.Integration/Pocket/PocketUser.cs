using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Integration.Pocket
{
    public class PocketUser
    {
        public string UserName { get; private set; }
        public string AccessCode { get; private set; }

        public PocketUser(string userName, string accessCode)
        {
            UserName = userName;
            AccessCode = accessCode;
        }
    }
}
