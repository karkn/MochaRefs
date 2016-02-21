using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Mocha.Refs.Core.Repositories;
using Mocha.Refs.Core.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Mocha.Refs.Testing;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using Mocha.Refs.Testing.Mock;

namespace Mocha.Refs.Core.Handlers
{
    [TestClass]
    public class TagHandlerTest
    {
        [TestMethod]
        public async Task TestGetAllTagsAsync_NameAscending()
        {
            var data = new List<Tag> 
            { 
                new Tag { Name = "BBB" }, 
                new Tag { Name = "ZZZ" }, 
                new Tag { Name = "AAA" }, 
            };
            var mockSet = DbSetMockBuilder.BuildAsyncMock(data);

            var mockContext = new Mock<IRefsContext>();
            mockContext.Setup(c => c.Tags).Returns(mockSet.Object);

            var handler = new TagHandler(mockContext.Object);
            var ret = (await handler.GetAllTagsAsync(Transfer.TagSortKind.NameAscending)).ToList();

            Assert.AreEqual(3, ret.Count);
            Assert.AreEqual("AAA", ret[0].Name);
            Assert.AreEqual("BBB", ret[1].Name);
            Assert.AreEqual("ZZZ", ret[2].Name);
        }

        [TestMethod]
        public async Task TestGetTagAsync()
        {
            var data = new List<Tag> 
            { 
                new Tag { Name = "BBB", Id = 0, }, 
                new Tag { Name = "ZZZ", Id = 1, },
                new Tag { Name = "AAA", Id = 2, }, 
            };
            var mockSet = DbSetMockBuilder.BuildAsyncMock(data);

            var mockContext = new Mock<IRefsContext>();
            mockContext.Setup(c => c.Tags).Returns(mockSet.Object);

            var handler = new TagHandler(mockContext.Object);
            var ret = await handler.GetTagAsync("ZZZ");

            Assert.AreEqual(1, ret.Id);
        }

    }
}
