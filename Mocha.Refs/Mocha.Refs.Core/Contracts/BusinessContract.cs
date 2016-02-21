using Microsoft.Practices.Unity;
using Mocha.Common.Exceptions;
using Mocha.Common.Unity;
using Mocha.Refs.Core.DataTypes;
using Mocha.Refs.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Contracts
{
    public static class BusinessContract
    {
        public static void Require(bool cond, Error error, params string[] parameters)
        {
            if (!cond)
            {
                throw new BusinessException(error, parameters);
            }
        }

        public static void Ensure(bool cond, Error error, params string[] parameters)
        {
            if (!cond)
            {
                throw new BusinessException(error, parameters);
            }
        }

        public static void Validate(bool cond, Error error, params string[] parameters)
        {
            if (!cond)
            {
                throw new BusinessException(error, parameters);
            }
        }

        public static void Raise(Error error, params string[] parameters)
        {
            throw new BusinessException(error, parameters);
        }

        public static void ValidateRowVersion(byte[] rv1, byte[] rv2)
        {
            if (rv1 == null || rv2 == null)
            {
                throw new ArgumentNullException();
            }
            if (!rv1.SequenceEqual(rv2))
            {
                throw new BusinessException(Errors.UpdateConcurrency);
            }
        }

        public static void ValidateWritePermission(long? userId)
        {
            var container = MochaContainer.GetContainer();
            var userContext = container.Resolve<IUserContext>();
            if (userContext.GetUser().Id != userId)
            {
                throw new BusinessException(Errors.InvalidUserWrite, Convert.ToString(userId));
            }
        }

    }
}
