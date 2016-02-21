using AutoMapper;
using Mocha.Common.Cache;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Mocha.Refs.Web.Readers
{
    public class RefListViewModelReader
    {
        private IRefListHandler _refListHandler;
        private IObjectCache<RefListViewModel> _refListViewModelCache;
        private IObjectCache<ICollection<RefViewModel>> _refViewModelsCache;

        public RefListViewModelReader(
            IRefListHandler refListHandler,
            IObjectCache<RefListViewModel> refListViewModelCache,
            IObjectCache<ICollection<RefViewModel>> refViewModelsCache
        )
        {
            _refListHandler = refListHandler;
            _refListViewModelCache = refListViewModelCache;
            _refViewModelsCache = refViewModelsCache;

        }

        public async Task<RefListViewModel> ReadRefListViewModel(long refListId)
        {
            var cacheKey = refListId.ToString();
            if (!_refListViewModelCache.Exists(cacheKey))
            {
                await UpdateCache(refListId);
            }
            return _refListViewModelCache.Get(cacheKey);
        }

        public async Task<ICollection<RefViewModel>> ReadRefViewModels(long refListId)
        {
            var cacheKey = refListId.ToString();
            if (!_refViewModelsCache.Exists(cacheKey))
            {
                await UpdateCache(refListId);
            }
            return _refViewModelsCache.Get(cacheKey);
        }

        private async Task UpdateCache(long id)
        {
            var cacheKey = id.ToString();
            var list = await _refListHandler.GetRefListAsync(id);
            if (list != null)
            {
                var listViewModel = Mapper.Map<RefListViewModel>(list);
                var refViewModels = Mapper.Map<ICollection<RefViewModel>>(list.Refs);
                _refListViewModelCache.Add(cacheKey, listViewModel, 10);
                _refViewModelsCache.Add(cacheKey, refViewModels, 10);
            }
        }
    }
}