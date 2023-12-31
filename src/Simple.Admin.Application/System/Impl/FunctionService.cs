﻿using System.Data;
using System.Linq.Expressions;

using AutoMapper;

using Simple.Admin.Application.Contracts.System.Models.Function;
using Simple.Admin.Domain.Entities.System.Enum;
using Simple.Admin.Domain.Shared.Core;

namespace Simple.Admin.Application.System.Impl
{
    public class FunctionService : IFunctionService, IScoped
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUser _miUser;
        private readonly IRepository<SysFunction> _functionRepo;
        private readonly IMemoryCache _cache;

        public FunctionService(IMapper mapper, ICurrentUser miUser
            , IRepository<SysFunction> functionRepo
            , IMemoryCache cache)
        {
            _mapper = mapper;
            _miUser = miUser;
            _functionRepo = functionRepo;
            _cache = cache;
        }

        public Task<MessageModel> AddFunctionAsync(FunctionOperation operation)
        {
            return AddOrUpdateFunctionAsync(operation);
        }

        public Task<MessageModel> UpdateFunctionAsync(FunctionOperation operation)
        {
            return AddOrUpdateFunctionAsync(operation);
        }

        public async Task<MessageModel<IList<SysFunctionFull>>> GetFunctionList(FunctionDto dto)
        {
            var exp = PredicateBuilder.Instance.Create<SysFunction>()
                .AndIf(!string.IsNullOrEmpty(dto.Title), x => x.Title.Contains(dto.Title!));
            var raw = await _functionRepo.GetListAsync(exp);
            var list = _mapper.Map<List<SysFunctionFull>>(raw.OrderBy(x => x.Sort));
            return new MessageModel<IList<SysFunctionFull>>(list);
        }

        private IList<SysFunctionFull> _allFunctions => GetFunctionsCacheAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        private async Task<MessageModel> AddOrUpdateFunctionAsync(FunctionOperation operation)
        {
            if (operation.Id <= 0)
            {
                var func = _mapper.Map<SysFunction>(operation);
                func.CreatedBy = _miUser.UserId;
                func.CreatedOn = DateTime.Now;
                func.Id = SnowflakeIdHelper.Next();
                await _functionRepo.AddAsync(func);
            }
            else
            {
                var func = _mapper.Map<SysFunction>(operation);
                func.Icon = operation.Icon;
                func.Title = operation.Title;
                func.Name = operation.Name;
                func.FrameSrc = operation.FrameSrc;
                func.Url = operation.Url;
                func.AuthorizationCode = operation.AuthorizationCode;
                func.ParentId = operation.ParentId;
                func.Sort = operation.Sort;
                func.FunctionType = (function_type)operation.FunctionType;
                await _functionRepo.UpdateAsync(func);
            }
            RemoveCache();

            return Back.Success();
        }

        public async Task<MessageModel<IList<FunctionItem>>> GetFunctionTreeAsync()
        {
            var searchList = _allFunctions.OrderBy(x => x.Sort);
            var topLevel = _allFunctions.Where(x => x.ParentId <= 0).OrderBy(x => x.Sort);
            var list = topLevel.Select(x => new FunctionItem
            {
                Title = x.Title,
                Icon = x.Icon,
                Url = x.Url,
                FunctionType = x.FunctionType,
                AuthorizationCode = x.AuthorizationCode,
                ParentId = x.ParentId,
                Sort = x.Sort,
                Id = x.Id,
                Children = GetFuncChildNode(x.Id)
            }).ToList();

            return new MessageModel<IList<FunctionItem>>(await Task.FromResult(list));
        }

        private IList<FunctionItem> GetFuncChildNode(long id)
        {
            var children = _allFunctions.Where(x => x.ParentId == id).OrderBy(x => x.Sort);
            return children.Select(x => new FunctionItem
            {
                Title = x.Title,
                Icon = x.Icon,
                Url = x.Url,
                FunctionType = x.FunctionType,
                AuthorizationCode = x.AuthorizationCode,
                ParentId = x.ParentId,
                Sort = x.Sort,
                Id = x.Id,
                Children = GetFuncChildNode(x.Id)
            }).ToList();
        }

        private IList<TreeOption> GetFunctionChildNode(long id)
        {
            var children = _allFunctions.Where(x => x.ParentId == id).OrderBy(x => x.Sort);
            return children.Select(x => new TreeOption
            {
                Name = x.Title,
                Value = x.Id.ToString(),
                Children = GetFunctionChildNode(x.Id)
            }).ToList();
        }

        public async Task<MessageModel> RemoveFunctionAsync(PrimaryKeys input)
        {
            if (input.array_id.IsNull()) return Back.Fail("id不能为空");

            var funcs = await _functionRepo.GetListAsync(x => input.array_id.Contains(x.Id));
            foreach (var item in funcs)
            {
                item.IsDeleted = 1;
            }
            await _functionRepo.UpdateRangeAsync(funcs);
            RemoveCache();

            return Back.Success();
        }

        public async Task<IList<SysFunctionFull>> GetFunctionsCacheAsync()
        {
            return await _cache.GetOrCreate(CacheConst.FUNCTION, async (entry) =>
            {
                var list = await _functionRepo.GetListAsync();
                _cache.Set(CacheConst.FUNCTION, list.ToList(), CacheConst.Week);
                return _mapper.Map<IList<SysFunctionFull>>(list);
            })!;
        }

        private void RemoveCache()
        {
            _cache.RemoveByPattern(StringHelper.UserFunctionCachePattern());
            _cache.Remove(CacheConst.FUNCTION);
        }

        public async Task<IList<string>> GetAllIdsAsync()
        {
            return (await GetFunctionsCacheAsync()).Select(x => x.Id.ToString()).ToList();
        }
    }
}