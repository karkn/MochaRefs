using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.UI.Test
{
    public class IisExpressDriver
    {
        private Process _iisProcess;
        private int _iisPort;

        public void Start(string applicationName, int iisPort)
        {
            _iisPort = iisPort;
            var applicationPath = GetApplicationPath(applicationName);
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            _iisProcess = new Process();
            _iisProcess.StartInfo.FileName = programFiles + @"\IIS Express\iisexpress.exe";
            _iisProcess.StartInfo.Arguments = string.Format(@"/path:{0} /port:{1}", applicationPath, _iisPort);
            _iisProcess.Start();
        }

        public void Stop()
        {
            if (_iisProcess != null && _iisProcess.HasExited == false)
            {
                _iisProcess.Kill();
            }
        }

        public string GetAbsoluteUrl(string relativeUrl = "/")
        {
            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }
            return String.Format("http://localhost:{0}{1}", _iisPort, relativeUrl);
        }

        private string GetApplicationPath(string applicationName)
        {
            var solutionFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
            return Path.Combine(solutionFolder, applicationName);
        }
    }
}
