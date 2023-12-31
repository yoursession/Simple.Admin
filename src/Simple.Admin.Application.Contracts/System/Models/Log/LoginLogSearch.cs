﻿namespace Simple.Admin.Application.Contracts.System.Models.Log
{
    public class LoginLogSearch : PagingSearchModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime[]? CreatedOn { get; set; }

        public int? Succeed { get; set; }
    }
}