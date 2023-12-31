﻿using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using Simple.Admin.Domain.Helper;
using Simple.Admin.Domain.Shared.Core;
using Simple.Admin.Domain.Shared.GlobalVars;
using Simple.Admin.Domain.Shared.Options;
using Simple.Admin.Domain.Shared.Response;

namespace Simple.Admin.Web.Host
{
    public class ValuesController : MiControllerBase
    {
        private readonly IQuickDict _dictionaryApi;

        public ValuesController(IQuickDict dictionaryApi)
        {
            _dictionaryApi = dictionaryApi;
        }

        [HttpGet]
        public MessageModel Init()
        {
            return new MessageModel<dynamic>(new
            {
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                tip = $"欢迎使用{SystemConfigConst.PROJECT_NAME}！",
                tip_star = "如果您感觉不错，记得点击下面链接给个star！",
                github = SystemConfigConst.GITHUB,
                gitee = SystemConfigConst.GITEE
            });
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> ExecuteAsync([FromBody] Option option)
        {
            var str = string.Empty;

            switch (option.Name)
            {
                case "字典value":
                    str = await _dictionaryApi.GetAsync(option.Value!);
                    break;

                case "今日日志":
                    var names = FileLogging.GetEveryDayLogs(DateTime.Now).Select(x => x.Name).ToList();
                    str = JsonSerializer.Serialize(names);
                    break;
            }

            return new MessageModel<string>(str);
        }

        [HttpGet]
        public FileResult CreateImageWithVerifyCode()
        {
            var bytes = DrawingHelper.CreateByteByImgVerifyCode("3456", 160, 40);
            return File(bytes, "image/png");
        }
    }
}