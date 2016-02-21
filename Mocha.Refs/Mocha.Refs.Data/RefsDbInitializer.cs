using Microsoft.Practices.Unity;
using Mocha.Common.Unity;
using Mocha.Refs.Core.DataTypes;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Data
{
    public class RefsDbInitializer : DropCreateDatabaseIfModelChanges<RefsContext>
    {
        public static void Init()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RefsContext, Configuration>());
            using (var context = new RefsContext())
            {
                context.Database.Initialize(false);
            }

            //Database.SetInitializer(new RefsDbInitializer());
            //using (var context = new RefsContext())
            //{
            //    var userContext = new RefsDbInitializerUserContext(context);
            //    var container = MochaContainer.GetContainer();
            //    container.RegisterInstance<IUserContext>(userContext);
            //    context.Database.Initialize(false);
            //}
        }

    //    private void CreateUserViewsFromAuthTable(RefsContext context)
    //    {
    //        // Entity Frameworkが自動で作ったテーブルを削除してViewを作成
    //        context.Database.ExecuteSqlCommand("DROP TABLE dbo.MochaUsers");
    //        context.Database.ExecuteSqlCommand("CREATE VIEW [dbo].[MochaUsers] AS SELECT CAST(Id AS UNIQUEIDENTIFIER) AS Id, UserName, Email FROM Auth.dbo.Users");
    //    }

    //    private void CreateIndexes(RefsContext context)
    //    {
    //        context.Database.ExecuteSqlCommand("CREATE INDEX IX_Tag_Name ON Tags (name)");
    //    }

    //    private MochaUser GetDefaultMochaUser(RefsContext context)
    //    {
    //        return (
    //            from u in context.MochaUsers
    //            where u.UserName == "mctest1"
    //            select u
    //        ).SingleOrDefault();
    //    }

    //    protected override void Seed(RefsContext context)
    //    {
    //        CreateUserViewsFromAuthTable(context);

    //        var mochaUser = GetDefaultMochaUser(context);
    //        var user = new User()
    //        {
    //            UserName = mochaUser.UserName,
    //            Email = mochaUser.Email,
    //            MochaUserId = mochaUser.Id,
    //        };
    //        context.Users.Add(user);
    //        // RefsDbInitializerUserContext.GetUserId()で値が取れるようにDBに反映しておく
    //        // この前にIAuditableなエンティティをcontextにAddしてはいけない
    //        context.SaveChanges();

    //        //var unfiledRefList = new RefList()
    //        //{
    //        //    Kind = RefListKind.Unfiled,
    //        //    Author = user,
    //        //    Title = "unfiled",
    //        //    Comment = "",
    //        //    PublishingStatus = PublishingStatusKind.Private,
    //        //    Refs = new List<Ref>(),
    //        //    Statistics = null,
    //        //};
    //        //for (var i = 0; i < 5; ++i) {
    //        //    var refe = new Ref()
    //        //    {
    //        //        Uri = "http://www.example.com/foo",
    //        //        Title = "未整理" + i,
    //        //        Comment = "未整理" + i  + "です",
    //        //        DisplayOrder = i,
    //        //    };
    //        //    unfiledRefList.Refs.Add(refe);
    //        //}
    //        //context.RefLists.Add(unfiledRefList);

    //        var tag = new Tag()
    //        {
    //            Name = "タグ1",
    //        };
    //        context.Tags.Add(tag);
    //        var tagUse = new TagUse()
    //        {
    //            Owner = user,
    //            Tag = tag,
    //        };

    //        tag = new Tag()
    //        {
    //            Name = "タグ2",
    //        };
    //        context.Tags.Add(tag);


    //        var refList = new RefList()
    //        {
    //            Title = "サンプルリンク集",
    //            Comment = "サンプルです。",
    //            Refs = new List<Ref>(),
    //            Author = user,
    //            Statistics = new RefListStatistics(),
    //        };
    //        refList.TagUses = new[] { tagUse };

    //        refList.Refs.Add(
    //            new Ref()
    //            {
    //                Kind = RefKind.Heading,
    //                Title = "見出し1",
    //                DisplayOrder = 0,
    //            }
    //        );
    //        refList.Refs.Add(
    //            new Ref()
    //            {
    //                Title = "foo",
    //                Uri = "http://example.com/foo",
    //                Comment = "fooです",
    //                DisplayOrder = 1,
    //            }
    //        );
    //        refList.Refs.Add(
    //            new Ref()
    //            {
    //                Title = "bar",
    //                Uri = "http://example.com/bar",
    //                Comment = "barです",
    //                DisplayOrder = 2,
    //            }
    //        );
    //        refList.Refs.Add(
    //            new Ref()
    //            {
    //                Title = "baz",
    //                Uri = "http://example.com/baz",
    //                Comment = "bazです",
    //                DisplayOrder = 3,
    //            }
    //        );

    //        refList.Refs.Add(
    //            new Ref()
    //            {
    //                Kind = RefKind.Heading,
    //                Title = "見出し2",
    //                DisplayOrder = 4,
    //            }
    //        );
    //        refList.Refs.Add(
    //            new Ref()
    //            {
    //                Title = "afo",
    //                Uri = "http://example.com/afo",
    //                Comment = "afoです",
    //                DisplayOrder = 5,
    //            }
    //        );
    //        refList.Refs.Add(
    //            new Ref()
    //            {
    //                Title = "bfo",
    //                Uri = "http://example.com/bfo",
    //                Comment = "bfoです",
    //                DisplayOrder = 6,
    //            }
    //        );

    //        context.RefLists.Add(refList);


    //        for (int i = 2; i < 100; ++i)
    //        {
    //            refList = new RefList()
    //            {
    //                Title = "サンプルリンク集" + i,
    //                Comment = "サンプルです。",
    //                Refs = new List<Ref>(),
    //                Author = user,
    //                Statistics = new RefListStatistics(),
    //            };
    //            refList.Refs.Add(
    //                new Ref()
    //                {
    //                    Title = "foo",
    //                    Uri = "http://example.com/foo",
    //                    Comment = "fooです",
    //                    DisplayOrder = 1,
    //                }
    //            );
    //            context.RefLists.Add(refList);
    //        }
            
    //        context.SaveChanges();

    //        base.Seed(context);
    //    }
    }
}
