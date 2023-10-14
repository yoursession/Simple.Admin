﻿using System.Collections.Concurrent;
using System.Reflection;

using Mi.Domain.DataAccess;
using Mi.Domain.Entities.System;
using Mi.Domain.PipelineConfiguration;
using Mi.Domain.Shared.Core;
using Mi.Domain.Shared.GlobalVars;
using Mi.Domain.Shared.Options;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mi.Domain.Services
{
#pragma warning disable CS8602 // 解引用可能出现空引用。

    public class DictionaryService : IDictionaryApi
    {
        private ConcurrentDictionary<string, string> _keyValuePairs;
        private List<SysDict> _sysDict;
        private readonly ILogger<DictionaryService> _logger;
        private readonly IMemoryCache _memoryCache;

        public DictionaryService(ILogger<DictionaryService> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public Task<string> GetAsync(string key)
        {
            if (_keyValuePairs == null) Load();
            return Task.FromResult(_keyValuePairs[key]);
        }

        public Task<Dictionary<string, string>> GetManyAsync(string parentKey)
        {
            if (_sysDict == null) Load();
            var dict = new Dictionary<string, string>();
            foreach (var item in _sysDict!.Where(x => x.ParentKey == parentKey))
            {
                dict.TryAdd(item.Key, item.Value ?? "");
            }
            return Task.FromResult(dict);
        }

        public Task<T> GetManyAsync<T>(string parentKey)
        {
            if (_sysDict == null) Load();
            var model = Activator.CreateInstance<T>();

            var children = _sysDict!.Where(x => x.ParentKey == parentKey).ToList();
            if (children.Count == 0) return Task.FromResult(model);
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                var item = children.FirstOrDefault(x => x.Key == prop.Name);
                if (item != null)
                {
                    prop.SetValue(model, Convert.ChangeType(item.Value, prop.PropertyType));
                }
            }

            return Task.FromResult(model);
        }

        public Task<List<Option>> GetOptionsAsync(string parentKey)
        {
            if (_sysDict == null) Load();

            var list = _sysDict!.Where(x => x.ParentKey == parentKey).Select(x => new Option
            {
                Name = x.Name,
                Value = x.Value
            }).ToList();

            return Task.FromResult(list);
        }

        public Task<bool> SetAsync(string key, string value)
        {
            return SetAsync(new Dictionary<string, string>
            {
                { key,value }
            });
        }

        public async Task<bool> SetAsync(Dictionary<string, string> dict)
        {
            var updateList = new List<SysDict>();
            var now = DateTime.Now;
            foreach (var item in dict)
            {
                var temp = _sysDict!.FirstOrDefault(d => d.Key == item.Key);

                if (temp == null) continue;

                temp.Value = item.Value;
                temp.ModifiedBy = -1;
                temp.ModifiedOn = now;
                updateList.Add(temp);
            }

            using (var p = ServiceManager.Provider.CreateScope())
            {
                var dictRepo = p.ServiceProvider.GetRequiredService<IRepository<SysDict>>();
                var res = 0 < await dictRepo.UpdateRangeAsync(updateList);
                if (res)
                {
                    Load();
                }
                return res;
            }
        }

        private void Load()
        {
            _keyValuePairs = new ConcurrentDictionary<string, string>();
            using (var p = ServiceManager.Provider.CreateScope())
            {
                var dictRepo = p.ServiceProvider.GetRequiredService<IRepository<SysDict>>();
                var allDict = dictRepo.GetListAsync(x => x.IsDeleted == 0).ConfigureAwait(false).GetAwaiter().GetResult();
                _sysDict = allDict;
                _memoryCache.Set(CacheConst.DICT, _sysDict);
                foreach (var kv in _sysDict)
                {
                    _keyValuePairs.TryAdd(kv.Key, kv.Value ?? "");
                }
                _logger.LogInformation("字典加载完毕");
            }
        }
    }
}