using AutoMapper;
using Mocha.Refs.Core.Transfer;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Web.Models;
using Mocha.Refs.Web.Models.List;
using Mocha.Refs.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mocha.Refs.Web.Helpers;

namespace Mocha.Refs.Web
{
    public static class MapConfig
    {
        public static void CreateMaps()
        {
            // View <= Service
            Mapper.CreateMap<Ref, RefViewModel>();
            Mapper.CreateMap<User, UserViewModel>().
                ForMember(
                    vm => vm.HasProfileImage,
                    opts => opts.ResolveUsing(u => ProfileImageHelper.ExistsSmallProfileImage(u.Id.ToString(), u.UserName))
                );
            Mapper.CreateMap<Tag, TagViewModel>().
                ForMember(
                    vm => vm.RefListCount,
                    opts => opts.ResolveUsing(t => t.Statistics.RefListCount)
                )
                .ForMember(
                    vm => vm.FavoriteCount,
                    opts => opts.ResolveUsing(u => u.Statistics.FavoriteCount)
                );
            Mapper.CreateMap<TagUse, TagUseViewModel>()
                .ForMember(
                    vm => vm.RefListCount,
                    opts => opts.ResolveUsing(u => u.Statistics.RefListCount)
                )
                .ForMember(
                    vm => vm.PublishedRefListCount,
                    opts => opts.ResolveUsing(u => u.Statistics.PublishedRefListCount)
                );
            Mapper.CreateMap<RefList, RefListViewModel>().
                ForMember(
                    vm => vm.TagUses,
                    opts => opts.ResolveUsing(l => l.TagUses.Select(u => u.Name).OrderBy(s => s))
                );
            Mapper.CreateMap<RefListStatistics, RefListStatisticsViewModel>();
            Mapper.CreateMap<PagedRefLists, PagedRefListsViewModel>().
                ForMember(
                    vm => vm.PageIndex,
                    opts => opts.ResolveUsing(o => o.PageIndex + 1)
                );

            Mapper.CreateMap<PagedRefLists, Mocha.Refs.Web.Models.List.ManagePageViewModel>();

            // View => Service
            Mapper.CreateMap<RefViewModel, Ref>();
            Mapper.CreateMap<UserViewModel, User>();
            Mapper.CreateMap<RefListViewModel, RefList>();

            Mapper.CreateMap<CreatePageViewModel, CreateRefListRequest>();
        }
    }
}