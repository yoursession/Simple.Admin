﻿namespace Simple.Admin.Domain.Shared.Response
{
    /// <summary>
    /// MessageModel
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// 响应代码
        /// </summary>
        public response_type Code { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string? Message { get; set; }

        public MessageModel()
        { }

        public MessageModel(response_type code, string? message)
        {
            Code = code;
            Message = message;
        }

        public virtual bool IsOk()
        {
            return Code == response_type.Success;
        }
    }

    public class MessageModel<T> : MessageModel
    {
        public override bool IsOk()
        {
            return Code == response_type.Success && Result != null;
        }

        /// <summary>
        /// 响应数据
        /// </summary>
        public T? Result { get; set; }

        public MessageModel()
        { }

        public MessageModel(response_type code, string msg, T? result)
        {
            Code = code;
            Message = msg;
            Result = result;
        }

        public MessageModel(bool successed, string msg, T? result)
        {
            Code = successed ? response_type.Success : response_type.Fail;
            Message = msg;
            Result = result;
        }

        public MessageModel(bool successed, T? result)
        {
            Code = successed ? response_type.Success : response_type.Fail;
            Message = successed ? "操作成功" : "操作失败";
            Result = result;
        }

        public MessageModel(T? result)
        {
            Code = response_type.Success;
            Message = "success";
            Result = result;
        }
    }
}