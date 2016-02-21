using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocha.Refs.Web;
using Mocha.Refs.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mocha.Refs.Web.Controllers
{
    [TestClass]
    public class AttributeTest
    {
        /// <summary>
        /// HttpPostを持つのにAjaxOnlyもValidateAntiForgeryTokenもないアクションがないか。
        /// </summary>
        [TestMethod]
        public void TestAntiForgeryTokenAttribute()
        {
            var assembly = typeof(WebConsts).Assembly;
            var allControllerTypes = assembly.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type));
            var allControllerActions = allControllerTypes.SelectMany(type => type.GetMethods());

            var failingActions =
                from action in allControllerActions
                let attrs = Attribute.GetCustomAttributes(action)
                where
                    attrs.Any(a => a is HttpPostAttribute) &&
                    !attrs.Any(a => a is AjaxOnlyAttribute) &&
                    !attrs.Any(a => a is ValidateAntiForgeryTokenAttribute)
                select action;

            var message = string.Join(", ", failingActions.Select(m => m.DeclaringType.FullName + "." + m.Name));
            Assert.IsFalse(failingActions.Any(), message);
        }
    }
}
