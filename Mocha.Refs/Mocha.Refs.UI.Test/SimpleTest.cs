using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Diagnostics;
using System.IO;

namespace Mocha.Refs.UI.Test
{
    [TestClass]
    public class SimpleTest
    {
        private const string applicationName = "Mocha.Refs.Web";
        private const int IisPort = 2020;

        private static IisExpressDriver _iis;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _iis = new IisExpressDriver();
            _iis.Start(applicationName, IisPort);
        }
        
        [ClassCleanup]
        public static void ClassCleanup()
        {
            _iis.Stop();
        }

        [TestMethod]
        public void TestSearch()
        {
            var driver = new InternetExplorerDriver();

            var homeUrl = _iis.GetAbsoluteUrl("/");

            driver.Navigate().GoToUrl(homeUrl);
            driver.FindElement(By.Name("search")).Clear();
            driver.FindElement(By.Name("search")).SendKeys("entity framework");
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(n => n.Title.StartsWith("リストの検索"));

            Assert.AreEqual("リストの検索", driver.FindElement(By.CssSelector("h1.page-header")).Text);

            driver.Quit();
        }

    }
}
