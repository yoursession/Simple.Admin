﻿using Simple.Admin.Application.Contracts.System;
using Simple.Admin.Application.Contracts.System.Models.Dict;
using Simple.Admin.Domain.Shared.Core;

namespace Simple.Admin.ControllerLibrary.System
{
    [Authorize]
    public class DictController : MiControllerBase
    {
        private readonly IDictService _dictService;

        public DictController(IDictService dictService)
        {
            _dictService = dictService;
        }

        /// <summary>
        /// 字典列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeCode("System:Dict:Query")]
        public async Task<MessageModel<PagingModel<DictItem>>> GetDictList([FromBody] DictSearch search)
            => await _dictService.GetDictListAsync(search);

        /// <summary>
        /// 新增字典
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeCode("System:Dict:Add")]
        public async Task<MessageModel> AddAsync([FromBody] DictPlus operation)
            => await _dictService.AddAsync(operation);

        /// <summary>
        /// 更新字典
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeCode("System:Dict:Update")]
        public async Task<MessageModel> UpdateAsync([FromBody] DictEdit operation)
            => await _dictService.UpdateAsync(operation);

        /// <summary>
        /// 移除字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost, AuthorizeCode("System:Dict:Remove")]
        public async Task<MessageModel> RemoveDict([FromBody] PrimaryKeys input)
            => await _dictService.RemoveDictAsync(input);
    }
}