using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Exceptions {
    [Serializable]
    public class Error
    {
        public string Id { get; private set; }
        public string Message { get; private set; }

        public Error(string id, string message)
        {
            Id = id;
            Message = message;
        }
    }
}
