﻿using System.Diagnostics.CodeAnalysis;

namespace Simple.Admin.Application.Contracts.System.Models.Function
{
    public class FunctionOperation
    {
        public long Id { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        [NotNull]
        public string? Title { get; set; }
        public string? Name { get; set; }

        public string? FrameSrc { get; set; }   

        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 访问地址
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 功能类型
        /// </summary>
        public int FunctionType { get; set; }

        /// <summary>
        /// 授权码
        /// </summary>
        public string? AuthorizationCode { get; set; }

        public long ParentId { get; set; }
        public int Sort { get; set; }
    }
}