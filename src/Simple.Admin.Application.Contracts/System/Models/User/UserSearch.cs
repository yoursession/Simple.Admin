﻿using Simple.Admin.Domain.Shared.Models;

namespace Simple.Admin.Application.Contracts.System.Models.User
{
    public class UserSearch : PagingSearchModel
    {
        public string? UserName { get; set; }
    }
}