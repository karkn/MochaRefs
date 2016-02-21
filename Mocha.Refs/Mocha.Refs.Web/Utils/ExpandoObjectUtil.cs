using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Utils
{
    public class ExpandoObjectUtil
    {
        public static bool HasProperty(ExpandoObject obj, string propertyName)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict.ContainsKey(propertyName);
        }

        public static object GetValue(ExpandoObject obj, string propertyName)
        {
            var dict = (IDictionary<string, object>)obj;
            return dict[propertyName];
        }

        public static object GetValueDefault(ExpandoObject obj, string propertyName, object defaultValue)
        {
            return HasProperty(obj, propertyName) ? GetValue(obj, propertyName) : defaultValue;
        }
    }
}