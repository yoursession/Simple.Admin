﻿using System.Diagnostics.CodeAnalysis;

namespace Simple.Admin.Application.Contracts.System.Models.Dict
{
    public class SysDictFull
    {
        public long Id { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        [NotNull]
        public string? Name { get; set; }

        /// <summary>
        /// 字典Key，唯一
        /// </summary>
        [NotNull]
        public string? Key { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// 父级Key
        /// </summary>
        public string? Type { get; set; }

        public int Sort { get; set; }
        public string? Remark { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}