using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Factories
{
    public static class TagUseFactory
    {
        public static TagUse Create(string name, long tagId, long ownerId)
        {
            return new TagUse()
            {
                Name = name,
                TagId = tagId,
                OwnerId = ownerId,
                Statistics = new TagUseStatistics(),
            };
        }
    }
}
