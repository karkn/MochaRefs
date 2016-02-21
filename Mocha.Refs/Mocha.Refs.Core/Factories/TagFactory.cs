using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Factories
{
    public static class TagFactory
    {
        public static Tag Create(string name)
        {
            return new Tag()
            {
                Name = name,
                Statistics = new TagStatistics(),
            };
        }
    }
}
