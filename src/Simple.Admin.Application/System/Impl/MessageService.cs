﻿using Dapper;

using Simple.Admin.Application.Contracts.System.Models.Message;
using Simple.Admin.Domain.Shared.Core;

namespace Simple.Admin.Application.System.Impl
{
    public class MessageService : IMessageService, IScoped
    {
        private readonly IRepository<SysMessage> _messageRepository;
        private readonly ICurrentUser _miUser;
        private readonly IDapperRepository _dapperRepository;

        public MessageService(IRepository<SysMessage> messageRepository, ICurrentUser miUser
            , IDapperRepository dapperRepository)
        {
            _messageRepository = messageRepository;
            _miUser = miUser;
            _dapperRepository = dapperRepository;
        }

        public async Task<MessageModel<IList<HeaderMsg>>> GetHeaderMsgAsync()
        {
            var list = (await _messageRepository.GetListAsync(x => x.Readed == 0 && x.ReceiveUser == _miUser.UserId)).OrderByDescending(x => x.CreatedOn);
            var result = new List<HeaderMsg>();
            var msg = new HeaderMsg
            {
                Title = "通知",
                Id = 1,
                Children = list.Select(x => new HeaderMsgChild
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = ShowContent(x.Content),
                    Time = ShowTime(x.CreatedOn)
                }).ToList()
            };
            result.Add(msg);
            return new MessageModel<IList<HeaderMsg>>(result);
        }

        private string ShowContent(string? content)
        {
            if (string.IsNullOrEmpty(content)) return "无内容";
            else if (content.Length >= 25) return content[..25] + "...";
            else return content;
        }

        private string ShowTime(DateTime time)
        {
            var now = DateTime.Now;
            var val = now.Subtract(time);
            if (val.TotalSeconds <= 60) return "刚刚";
            else if (val.TotalHours < 1) return $"{Math.Ceiling(val.TotalMinutes)}分钟前";
            else if (val.TotalHours >= 1 && val.TotalHours <= 23) return $"{Math.Ceiling(val.TotalHours)}小时前";
            else return $"{Math.Ceiling(val.TotalDays)}天前";
        }

        public async Task<MessageModel<PagingModel<SysMessageFull>>> GetMessageListAsync(MessageSearch search)
        {
            var sql = "select * from SysMessage where IsDeleted=0 and ReceiveUser=@userId";
            var parameter = new DynamicParameters();
            parameter.Add("userId", _miUser.UserId);
            if (!string.IsNullOrEmpty(search.Vague))
            {
                sql += " and (Title like @vague or Content like @vague) ";
                parameter.Add("vague", "%" + search.Vague + "%");
            }
            if (search.No.HasValue && search.No.Value > 0)
            {
                sql += " and Id=@id";
                parameter.Add("id", search.No.GetValueOrDefault());
            }
            if (search.Readed.HasValue && search.Readed >= 0)
            {
                sql += " and Readed=@readed";
                parameter.Add("readed", search.Readed.GetValueOrDefault());
            }
            if (!string.IsNullOrEmpty(search.WriteTime) && search.WriteTime.Contains("~"))
            {
                var v1 = DateTime.TryParse(search.WriteTime.Split("~")[0], out var start);
                var v2 = DateTime.TryParse(search.WriteTime.Split("~")[1], out var end);
                if (v1 && v2)
                {
                    sql += " and CreatedOn >= @start and CreatedOn <= @end";
                    parameter.Add("start", start.Date.ToString("yyyy-MM-dd"));
                    parameter.Add("end", end.Date.ToString("yyyy-MM-dd") + " 23:59:59");
                }
            }

            var result = await _dapperRepository.QueryPagedAsync<SysMessageFull>(sql, search.Page, search.Size, " Readed asc,CreatedOn desc ", parameter);
            return new MessageModel<PagingModel<SysMessageFull>>(result);
        }

        public async Task<MessageModel> ReadedAsync(PrimaryKeys input)
        {
            if (input.array_id.IsNull()) return Back.ParameterError(nameof(input.array_id));
            await _dapperRepository.ExecuteAsync("update SysMessage set ModifiedOn=@time,ModifiedBy=@user,Readed=1 where Id in @ids", new { time = DateTime.Now, user = _miUser.UserId, ids = input.array_id });
            return Back.Success();
        }
    }
}