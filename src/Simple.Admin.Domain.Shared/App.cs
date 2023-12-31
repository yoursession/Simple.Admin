﻿using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace Simple.Admin.Domain.Shared
{
    public sealed class App
    {
        /// <summary>
        /// 是否开发环境
        /// </summary>
        public static bool IsDevelopment { get; private set; }

        /// <summary>
        /// wwwroot磁盘绝对地址
        /// </summary>
        public static string WebRoot { get; private set; }

        /// <summary>
        /// appsettings.json配置
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        /// 服务提供
        /// </summary>
        public static IServiceProvider Provider { get; private set; }

        /// <summary>
        /// App.cs随着.net容器运行
        /// </summary>
        /// <param name="isDev">是否开发环境</param>
        /// <param name="webRoot">wwwroot磁盘绝对地址</param>
        /// <param name="configuration">appsettings.json配置读写</param>
        /// <param name="serviceProvider">服务提供者</param>
        public static void Running(bool isDev, string webRoot, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            IsDevelopment = isDev;
            WebRoot = webRoot;
            Configuration ??= configuration;
            Provider ??= serviceProvider;
        }

        /// <summary>
        /// 自动配置管道时加载程序集
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> LoadAssemblies()
        {
            var list = new List<Assembly>
            {
                Assembly.Load("Simple.Admin.Application"),
                Assembly.Load("Simple.Admin.Application.Contracts"),
                Assembly.Load("Simple.Admin.DataDriver"),
                Assembly.Load("Simple.Admin.Domain"),
                Assembly.Load("Simple.Admin.ControllerLibrary"),
                Assembly.Load("Simple.Admin.Web.Host")
            };

            return list;
        }
    }
}