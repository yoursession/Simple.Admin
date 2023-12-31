﻿using Simple.Admin.Application.Contracts.System.Models.Function;

namespace Simple.Admin.Application.Contracts.System
{
    public interface IFunctionService
    {
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        Task<MessageModel<IList<SysFunctionFull>>> GetFunctionList(FunctionDto dto);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        Task<MessageModel> AddFunctionAsync(FunctionOperation operation);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        Task<MessageModel> UpdateFunctionAsync(FunctionOperation operation);

        /// <summary>
        /// 列表（带树形）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<MessageModel<IList<FunctionItem>>> GetFunctionTreeAsync();

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MessageModel> RemoveFunctionAsync(PrimaryKeys input);

        /// <summary>
        /// 所有功能（带缓存）
        /// </summary>
        /// <returns></returns>
        Task<IList<SysFunctionFull>> GetFunctionsCacheAsync();

        /// <summary>
        /// 所有功能ID
        /// </summary>
        /// <returns></returns>
        Task<IList<string>> GetAllIdsAsync();
    }
}