﻿using Simple.Admin.Application.Contracts.System;
using Simple.Admin.Application.Contracts.System.Models.Tasks;
using Simple.Admin.Domain.Shared.Core;

namespace Simple.Admin.ControllerLibrary.System
{
    [Authorize]
    public class SysTaskController : MiControllerBase
    {
        private readonly ISysTaskService _sysTaskService;

        public SysTaskController(ISysTaskService sysTaskService)
        {
            _sysTaskService = sysTaskService;
        }

        /// <summary>
        /// 定时任务列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeCode("System:SysTask:GetList")]
        public Task<MessageModel<List<TaskItem>>> GetListAsync()
        {
            return _sysTaskService.GetListAsync();
        }

        /// <summary>
        /// 更新定时任务配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeCode("System:SysTask:Update")]
        public Task<MessageModel> UpdateAsync([FromBody] TaskEdit input)
        {
            return _sysTaskService.UpdateAsync(input);
        }
    }
}