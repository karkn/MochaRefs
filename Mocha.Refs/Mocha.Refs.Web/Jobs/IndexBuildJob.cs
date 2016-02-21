using Mocha.Common.Unity;
using Mocha.Refs.Core.Handlers;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Jobs
{
    public class IndexBuildJob : IJob
    {
        private static Logger _logger = LogManager.GetLogger("Index");

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("Begin building index");

            try
            {
                var handler = MochaContainer.Resolve<ISystemHandler>();
                handler.BuildIndex();

                _logger.Info("Finish building index");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed building index" + ex.ToString());
            }
        }
    }

}