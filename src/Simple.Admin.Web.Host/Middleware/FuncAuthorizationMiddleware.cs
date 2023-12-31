﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

using Simple.Admin.Domain.Extension;
using Simple.Admin.Domain.Shared.Attributes;
using Simple.Admin.Domain.Shared.Response;

namespace Simple.Admin.Web.Host.Middleware
{
    public class FuncAuthorizationMiddleware : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
        private readonly ILogger<FuncAuthorizationMiddleware> _logger;
        private readonly string[] IGNORE_PAGES = ["/entry"];
        private readonly List<Type> CONTROLLER_TYPES = [typeof(Controller), typeof(ControllerBase)];
        private readonly List<string?> VIEW_TYPES = [typeof(IActionResult).FullName, typeof(ViewResult).FullName, typeof(RedirectResult).FullName];

        public FuncAuthorizationMiddleware(ILogger<FuncAuthorizationMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            var userModel = context.GetUser();
            var pageRoute = (string?)context.Request.RouteValues["page"] ?? "";

            if (!IGNORE_PAGES.Contains(pageRoute.ToLower()) && userModel != null && userModel.UserId > 0)
            {
                var path = context.Request.Path.Value;
                if (!userModel.IsSuperAdmin)
                {
                    var flag = false;
                    if (userModel.PowerItems != null)
                    {
                        var endpoint = context.GetEndpoint();
                        var attr = endpoint?.Metadata.GetMetadata<AuthorizeCodeAttribute>();
                        if (endpoint != null)
                        {
                            if (attr != null) flag = userModel.PowerItems!.Any(x => x.AuthCode == attr.Code);
                            else flag = true;
                        }
                    }
                    if (!flag)
                    {
                        _logger.LogWarning($"用户Id：{userModel.UserId}，用户名：{userModel.UserName}'访问地址`{path}`权限不足");

                        if (!pageRoute.IsNull())
                        {
                            context.Response.Redirect("/html/403.html");
                            return;
                        }

                        await context.Response.WriteAsJsonAsync(new MessageModel(response_type.Forbidden, "权限不足，无法访问或操作"));
                        return;
                    }
                }
            }
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}