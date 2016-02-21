using Mocha.Common.Unity;
using Mocha.Refs.Core.Handlers;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Mocha.Refs.Web.Jobs
{
    public class UpdateTagStatisticsJob : IJob
    {
        private static Logger _logger = LogManager.GetLogger("TagStatistics");

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("Begin update tag statistics");

            try
            {
                var handler = MochaContainer.Resolve<ISystemHandler>();
                handler.UpdateTagStatistics();

                _logger.Info("Finish updating tag statistics");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed updating tag statistics" + ex.ToString());
            }
        }
    }

}