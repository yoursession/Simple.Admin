﻿using Simple.Admin.Application.Contracts.Public;
using Simple.Admin.Application.Contracts.Public.Models;
using Simple.Admin.Domain.Shared.Core;

namespace Simple.Admin.ControllerLibrary.Account
{
    [AllowAnonymous]
    public class RefreshTokenController : MiControllerBase
    {
        private readonly IPublicService _publicService;

        public RefreshTokenController(IPublicService publicService)
        {
            _publicService = publicService;
        }

        [HttpPost]
        public MessageModel Post(string t)
        {
            return _publicService.GetRefreshTokenResult(new RefreshTokenDto { Token = t });
        }
    }
}