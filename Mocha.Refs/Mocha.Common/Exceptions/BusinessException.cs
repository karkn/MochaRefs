using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Exceptions
{
    [Serializable]
    public class BusinessException: Exception
    {
        public Error Error { get; private set; }
        public string[] Parameters { get; private set; }

        public BusinessException(Error error, params string[] parameters)
        {
            Error = error;
            Parameters = parameters;
        }

        public BusinessException(Exception innerException, Error error, params string[] parameters)
            : base(string.Empty, innerException)
        {
            Error = error;
            Parameters = parameters;
        }

        public override string Message
        {
            get { return string.Format(Error.Message, Parameters); }
        }
    }
}
