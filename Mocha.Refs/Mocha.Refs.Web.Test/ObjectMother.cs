using Microsoft.Practices.Unity;
using Mocha.Refs.Core;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Web
{
    public class ObjectMother
    {
        public static string GetRefListTitle()
        {
            return "タイトル1";
        }
        public static string GetRefListUri()
        {
            return "http://www.mochaware.jp";
        }
        public static string GetRefListComment()
        {
            return "コメント1";
        }

        public static ICollection<TagUse> GetTagUses()
        {
            return new List<TagUse> {
                new TagUse {
                    Id = 1,
                    Name = "tag1",
                },
            };
        }

        public static ICollection<RefList> GetRefLists()
        {
            return new List<RefList>()
            {
                new RefList() {
                    Id = 1,
                    Title = "リスト1",
                    TagUses = GetTagUses(),
                },
                new RefList() {
                    Id = 2,
                    Title = "リスト2",
                    TagUses = GetTagUses(),
                },
            };
        }

        public static PagedRefLists GetPagedRefLists()
        {
            return new PagedRefLists()
            {
                PageIndex = 1,
                PageCount = 1,
                RefLists = ObjectMother.GetRefLists(),
                AllRefListCount = 2,
            };
        }

        public static User GetUser()
        {
            return new User()
            {
                Id = 1,
                UserName = "test1",
                DisplayName = "テスト1",
            };
        }

        public static void RegisterAuthenticatedUserContext(IUnityContainer container)
        {
            container.RegisterType<IUserContext, AuthenticatedUserContext>();
        }

        public class AuthenticatedUserContext : IUserContext
        {
            public bool IsAuthenticated
            {
                get { return true; }
            }

            public User GetUser()
            {
                return ObjectMother.GetUser();
            }
        }
    }
}
