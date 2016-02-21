using Mocha.Refs.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Contracts
{
    public static class SystemContract
    {
        /// <summary>
        /// 事前チェック。
        /// </summary>
        public static void Require(bool cond, Func<Exception> exceptionProvider)
        {
            if (!cond)
            {
                throw exceptionProvider();
            }
        }

        /// <summary>
        /// 事後チェック。
        /// </summary>
        public static void Ensure(bool cond, Func<Exception> exceptionProvider)
        {
            if (!cond)
            {
                throw exceptionProvider();
            }
        }

        /// <summary>
        /// 汎用チェック。
        /// </summary>
        public static void Validate(bool cond, Func<Exception> exceptionProvider)
        {
            if (!cond)
            {
                throw exceptionProvider();
            }
        }

        public static void Require(bool cond, string message, params string[] parameters)
        {
            if (!cond)
            {
                throw new ArgumentException(string.Format(message, parameters));
            }
        }

        public static void Ensure(bool cond, string message, params string[] parameters)
        {
            if (!cond)
            {
                throw new SystemException(string.Format(message, parameters));
            }
        }

        public static void Validate(bool cond, string message, params string[] parameters)
        {
            if (!cond)
            {
                throw new SystemException(string.Format(message, parameters));
            }
        }

        public static void RequireNotNull(object value, string valueName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(valueName);
            }
        }

        public static void RequireEntityIdentity(EntityIdentity identity)
        {
            if (identity == null || identity.RowVersion == null)
            {
                throw new ArgumentNullException("identity");
            }
        }

        public static void RequireNotNullOrWhiteSpace(string value, string valueName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(valueName + "がnullまたは空白です");
            }
        }

    }
}
