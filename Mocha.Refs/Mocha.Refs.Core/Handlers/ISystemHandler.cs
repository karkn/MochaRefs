using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public interface ISystemHandler
    {
        void BuildIndex();
        void UpdateTagStatistics();
    }
}
